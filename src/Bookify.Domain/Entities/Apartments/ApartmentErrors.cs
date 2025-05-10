using Bookify.ShareKernel.Errors;

namespace Bookify.Domain.Entities.Apartments;

public static class ApartmentErrors
{
    // public static readonly Error NotFound = new(
    //     "Property.NotFound",
    //     "The property with the specified identifier was not found");
    //
    public static Error NotFound(Guid apartmentId) => Error.NotFound(
        "Property.NotFound",
        $"The property with the Id = '{apartmentId}' was not found");
}