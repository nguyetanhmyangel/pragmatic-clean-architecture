namespace Bookify.ShareKernel.Entities;

public interface IEntity
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}