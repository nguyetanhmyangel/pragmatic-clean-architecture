namespace Bookify.ShareKernel.Entities;

public abstract class Entity<TEntityId> : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
    protected Entity(TEntityId id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        Id = id;
    }

    //EF Migration usage
    protected Entity() { }

    public TEntityId Id { get; init; }

    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity<TEntityId>))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        var other = (Entity<TEntityId>)obj;

        if (Id == null || other.Id == null)
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    public static bool operator ==(Entity<TEntityId> left, Entity<TEntityId> right)
    {
        if (Equals(left, null))
            return Equals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TEntityId> left, Entity<TEntityId> right)
    {
        return !(left == right);
    }
}
