using Bookify.Domain.Entities.Apartments.Enums;
using Bookify.Domain.Entities.Apartments.ValueObjects;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Domain.Entities.Apartments;


public record ApartmentId(Guid Value);
public sealed class Apartment : Entity<ApartmentId>
{
    private readonly List<Amenity> _amenities = new();

    public Apartment(
        ApartmentId id,
        string name,
        string description,
        Address address,
        Money price,
        Money cleaningFee,
        List<Amenity> amenities)
        : base(id)
    {
        Name = name;
        Description = description;
        Address = address;
        Price = price;
        CleaningFee = cleaningFee;
        _amenities = amenities ?? new List<Amenity>();
    }

    private Apartment() : base() { }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Address Address { get; private set; }
    public Money Price { get; private set; }
    public Money CleaningFee { get; private set; }
    public DateTime? LastBookedOnUtc { get; internal set; }
    public IReadOnlyCollection<Amenity> Amenities => _amenities.AsReadOnly();

    public void AddAmenity(Amenity amenity)
    {
        if (amenity != null && !_amenities.Contains(amenity))
        {
            _amenities.Add(amenity);
        }
    }

    public void RemoveAmenity(Amenity amenity)
    {
        _amenities.Remove(amenity);
    }
}