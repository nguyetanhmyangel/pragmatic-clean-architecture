﻿using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Bookings.Enums;
using Bookify.Domain.Entities.Reviews.Events;
using Bookify.Domain.Entities.Reviews.ValueObjects;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Utilities;

namespace Bookify.Domain.Entities.Reviews;

public class Review : Entity<ReviewId>
{
    private Review(
        ReviewId id,
        ApartmentId apartmentId,
        BookingId bookingId,
        UserId userId,
        Rating rating,
        string comment,
        DateTime createdOnUtc) : base(id)
    {
        ApartmentId = apartmentId;
        BookingId = bookingId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
        CreatedOnUtc = createdOnUtc;
    }

    private Review() : base() { }

    public ApartmentId ApartmentId { get; private set; }
    public BookingId BookingId { get; private set; }
    public UserId UserId { get; private set; }
    public Rating Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static Result<Review> Create(
        Booking booking,
        Rating rating,
        string comment,
        DateTime createdOnUtc)
    {
        if (booking.Status != BookingStatus.Completed)
            return Result.Failure<Review>(ReviewErrors.NotEligible);

        var review = new Review(
            ReviewId.New(),
            booking.ApartmentId,
            booking.Id,
            booking.UserId,
            rating,
            comment,
            createdOnUtc);

        //review.RaiseDomainEvent(new ReviewCreatedDomainEvent(review.Id));
        return review;
    }
}