using Bookify.Domain.Entities.Apartments;
using Bookify.Infrastructure.Database;
using Bookify.Infrastructure.Repositories.Generic;

namespace Bookify.Infrastructure.Repositories;
internal sealed class ApartmentRepository(ApplicationDbContext dbContext)
    : GenericRepository<Apartment, ApartmentId>(dbContext), IApartmentRepository;
