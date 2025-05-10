using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Exceptions;
using Bookify.ShareKernel.Repositories;
using Bookify.ShareKernel.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using Bookify.Infrastructure.Outbox;

namespace Bookify.Infrastructure.Database;
public sealed class ApplicationDbContext(
    DbContextOptions options,
    IPublisher publisher,
    IDateTimeProvider dateTimeProvider)
    : DbContext(options), IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    /*
        1. Tham chiếu dự án:
            Tầng Infrastructure tham chiếu tầng Domain, cho phép ApplicationDbContext và các lớp cấu hình truy cập các lớp entity (như User).
        2. Cấu hình ánh xạ:
            Các lớp cấu hình (IEntityTypeConfiguration<T>) trong tầng Infrastructure định nghĩa cách ánh xạ các entity sang bảng cơ sở dữ liệu.
            modelBuilder.ApplyConfigurationsFromAssembly tự động tìm và áp dụng tất cả các cấu hình này.
        3. Không cần DbSet trực tiếp:
            Trong mã , ApplicationDbContext không định nghĩa các DbSet (như DbSet<User> Users) cho từng entity.
            Thay vào đó, các entity được nhận diện gián tiếp thông qua các cấu hình trong IEntityTypeConfiguration<T>.
            EF Core sẽ bao gồm tất cả các entity được cấu hình trong OnModelCreating vào mô hình, ngay cả khi chúng không có DbSet.
        4. Migration và schema:
            Khi chạy lệnh migration, EF Core sử dụng OnModelCreating để xây dựng mô hình cơ sở dữ liệu.
            Các entity từ tầng Domain (được tham chiếu bởi các lớp cấu hình) sẽ được chuyển thành các bảng, cột, và ràng buộc trong migration. 
     */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);
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
        foreach (var domainEvent in domainEvents) await publisher.Publish(domainEvent);
    }

    // private void AddDomainEventsAsOutboxMessages()
    // {
    //     var outboxMessages = ChangeTracker
    //         .Entries<IEntity>()
    //         .Select(entry => entry.Entity)
    //         .SelectMany(entity =>
    //         {
    //             var domainEvents = entity.GetDomainEvents();
    //             entity.ClearDomainEvents();
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
