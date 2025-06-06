﻿using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(bookingId => bookingId.Value, value => new BookingId(value));
        builder.OwnsOne(x => x.PriceForPeriod, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
        builder.OwnsOne(x => x.CleaningFee, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
        builder.OwnsOne(x => x.AmenitiesUpCharge, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
        builder.OwnsOne(x => x.TotalPrice, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
        builder.OwnsOne(x => x.Duration);
        builder.HasOne<Apartment>()
            .WithMany()
            .HasForeignKey(x => x.ApartmentId);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}
