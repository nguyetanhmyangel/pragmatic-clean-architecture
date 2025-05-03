using Bookify.ShareKernel.Event;

namespace Bookify.ShareKernel.BaseEntity;

public interface IEntity
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}