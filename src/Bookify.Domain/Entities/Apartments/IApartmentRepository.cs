using Bookify.ShareKernel.Repositories;

namespace Bookify.Domain.Entities.Apartments;

public interface IApartmentRepository:IRepository<Apartment, ApartmentId>
{
}