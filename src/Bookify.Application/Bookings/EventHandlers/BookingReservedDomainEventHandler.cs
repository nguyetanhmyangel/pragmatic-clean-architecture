﻿using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Bookings.Events;
using Bookify.Domain.Entities.Users;
using MediatR;

namespace Bookify.Application.Bookings.EventHandlers;

internal sealed class BookingReservedDomainEventHandler(
    IBookingRepository bookingRepository,
    IUserRepository userRepository,
    IEmailService emailService)
    : INotificationHandler<BookingReservedDomainEvent>
{
    // private readonly IBookingRepository _bookingRepository;
    // private readonly IUserRepository _userRepository;
    // private readonly IEmailService _emailService;
    //
    // public BookingReservedDomainEventHandler(
    //     IBookingRepository bookingRepository, 
    //     IUserRepository userRepository, 
    //     IEmailService emailService)
    // {
    //     _bookingRepository = bookingRepository;
    //     _userRepository = userRepository;
    //     _emailService = emailService;
    // }
    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (booking is null) return;

        var user = await userRepository.GetByIdAsync(booking.UserId, cancellationToken);
        if (user is null) return;

        await emailService.SendAsync(
            user.Email,
            "Booking reserved!",
            "You have 10 minutes to confirm this booking");
    }
}
