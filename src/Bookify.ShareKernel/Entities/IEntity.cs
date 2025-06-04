namespace Bookify.ShareKernel.Entities;

public interface IEntity
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}