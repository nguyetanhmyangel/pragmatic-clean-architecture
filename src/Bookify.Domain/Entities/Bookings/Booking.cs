﻿using Bookify.Domain.Entities.Bookings.Enums;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings.Events;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Utilities;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Domain.Entities.Bookings;

public class Booking : Entity<BookingId>
{
    private Booking(
        BookingId id,
        ApartmentId apartmentId,
        UserId userId,
        DateRange duration,
        Money priceForPeriod,
        Money cleaningFee,
        Money amenitiesUpCharge,
        Money totalPrice,
        BookingStatus status,
        DateTime createdOnUtc)
        : base(id)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        CleaningFee = cleaningFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    private Booking() : base() { }

    public ApartmentId ApartmentId { get; private set; }
    public UserId UserId { get; private set; }
    public DateRange Duration { get; private set; }
    public Money PriceForPeriod { get; private set; }
    public Money CleaningFee { get; private set; }
    public Money AmenitiesUpCharge { get; private set; }
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ConfirmedOnUtc { get; private set; }
    public DateTime? RejectedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public DateTime? CancelledOnUtc { get; private set; }

    public static Booking Reserve(Apartment apartment, UserId userId, DateRange duration, DateTime utcNow, PricingService pricingService)
    {
        var pricingDetails = pricingService.CalculatePrice(apartment, duration);
        var booking = new Booking(
            BookingId.New(),
            apartment.Id,
            userId,
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.CleaningFee,
            pricingDetails.AmenitiesUpCharge,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved,
            utcNow);

        //booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));
        apartment.LastBookedOnUtc = utcNow;
        return booking;
    }

    public Result Confirm(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.Confirmed;
        ConfirmedOnUtc = utcNow;
        //RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));
        return Result.Success();
    }

    public Result Reject(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.Rejected;
        RejectedOnUtc = utcNow;
        //RaiseDomainEvent(new BookingRejectedDomainEvent(Id));
        return Result.Success();
    }

    public Result Complete(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(BookingErrors.NotConfirmed);

        Status = BookingStatus.Completed;
        CompletedOnUtc = utcNow;
        //RaiseDomainEvent(new BookingCompletedDomainEvent(Id));
        return Result.Success();
    }

    public Result Cancel(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(BookingErrors.NotConfirmed);

        var currentDate = DateOnly.FromDateTime(utcNow);
        if (currentDate > Duration.Start)
            return Result.Failure(BookingErrors.AlreadyStarted);

        Status = BookingStatus.Cancelled;
        CancelledOnUtc = utcNow;
        //RaiseDomainEvent(new BookingCancelledDomainEvent(Id));
        return Result.Success();
    }
}