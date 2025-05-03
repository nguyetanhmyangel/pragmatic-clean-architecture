using Bookify.ShareKernel.BaseEntity;
using Bookify.ShareKernel.BaseRepository;
using Bookify.ShareKernel.Exceptions;
using Bookify.ShareKernel.Time;
//using Bookify.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IPublisher _publisher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions options, 
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider) : base(options)
    {
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Tự động quét toàn bộ Assembly (tức là toàn bộ project) chứa class ApplicationDbContext,
        //nếu class đó implement IEntityTypeConfiguration thì nó sẽ được quét và thực thi phương thức Configure
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            //First process the domain events and adding them to ChangeTracker as Outbox Messages,
            //then persisting everything in the database in a single transaction "atomic operation" 
            //AddDomainEventsAsOutboxMessages();
            var result = await base.SaveChangesAsync(cancellationToken);
            // await PublishDomainEventsAsync();
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception ocurred.", ex);
        }
        
    }
    
    //PublishDomainEventsAsync() gửi thẳng các domain events đi ngay lập tức.
    //Publishing an event can fail and is not in a transaction. Changing to Outbox pattern
    private async Task PublishDomainEventsAsync()
    {
        /*
         Using the ChangeTracker, we grab the entries which implement the Entity class. 
         Then we grab all the domain events and publish one by one.         
         */
        var domainEvents = ChangeTracker
            //Lấy tất cả các entity (thực thể) đang được EF Core theo dõi (ChangeTracker) mà implement interface IEntity.
            .Entries<IEntity>()
            //Lấy bản thân entity ra từ Entry
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                //Lấy danh sách các domain events từ entity
                var domainEvents = entity.GetDomainEvents();
                //Xóa danh sách domain events khỏi entity(tránh việc xử lý lặp lại sau này).
                entity.ClearDomainEvents();
                return domainEvents;
            }).ToList();
        //Gửi từng domain event ra ngoài (thường là gửi message đến bus như Kafka, RabbitMQ hoặc MediatR).
        foreach (var domainEvent in domainEvents) await _publisher.Publish(domainEvent);
    }

    // private void AddDomainEventsAsOutboxMessages()
    // {
    //     var outboxMessages = ChangeTracker
    //         .Entries<IEntity>()
    //         .Select(entry => entry.Entity)
    //         .SelectMany(entity =>
    //         {
    //             var domainEvents = entity.GetDomainEvents();
    //
    //             entity.ClearDomainEvents();
    //
    //             return domainEvents;
    //         })
    //         .Select(domainEvent => new OutboxMessage(
    //             Guid.NewGuid(),
    //             _dateTimeProvider.UtcNow,
    //             domainEvent.GetType().Name,
    //             JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
    //         .ToList();
    //
    //     AddRange(outboxMessages);
    // }
}
