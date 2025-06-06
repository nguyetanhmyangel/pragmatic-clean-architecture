﻿using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.Specifications;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.Exceptions;
using Bookify.ShareKernel.Repositories;
using Bookify.ShareKernel.Utilities;
using Bookify.ShareKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Bookings.Commands;

public sealed class ReserveBookingCommandHandler(
    IUserRepository userRepository,
    IApartmentRepository apartmentRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    PricingService pricingService,
    IDateTimeProvider dateProvider)
    : ICommandHandler<ReserveBookingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(new UserId(request.UserId), cancellationToken);
        if (user is null)
            return Result.Failure<Guid>(UserErrors.NotFound(user.Id.Value));

        var apartment = await apartmentRepository.GetByIdAsync(new ApartmentId(request.ApartmentId), cancellationToken);
        if (apartment is null)
            return Result.Failure<Guid>(ApartmentErrors.NotFound(apartment.Id.Value));

        var duration = DateRange.Create(request.StartDate, request.EndDate);
        
        // Sử dụng Specifications trong Handler
        var overlappingSpec = new GetBookingOverlapSpecification(apartment.Id, duration);

        var isOverlapping = await bookingRepository
            .AnyAsync(overlappingSpec, cancellationToken);

        if (isOverlapping)
            return Result.Failure<Guid>(BookingErrors.Conflict(apartment.Id.Value));
        try
        {
            var booking = Booking.Reserve(apartment, user.Id, duration, dateProvider.UtcNow, pricingService);
            bookingRepository.Add(booking);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return booking.Id.Value;
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(BookingErrors.Conflict(apartment.Id.Value));
        }        
    }
}
