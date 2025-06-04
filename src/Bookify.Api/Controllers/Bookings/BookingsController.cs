//using Asp.Versioning;

using Asp.Versioning;
using Bookify.Application.Bookings.Commands;
using Bookify.Application.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Bookings;

// [Authorize]
// [ApiController]
// [Route("api/bookings")]
//[ApiVersion(ApiVersions.V1)]
// [Route("api/v{version:apiVersion}/bookings")]
// public class BookingsController : ControllerBase
// {
//     private readonly ISender _sender;
//
//     public BookingsController(ISender sender)
//     {
//         _sender = sender;
//     }
//
//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetBooking(Guid id, CancellationToken cancellationToken)
//     {
//         var query = new GetBookingByIdQuery(id);
//
//         var result = await _sender.Send(query, cancellationToken);
//
//         return result.IsSuccess ? Ok(result.Value) : NotFound();
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> ReserveBooking(
//         ReserveBookingRequest request,
//         CancellationToken cancellationToken)
//     {
//         var command = new ReserveBookingCommand(
//             request.ApartmentId,
//             request.UserId,
//             request.StartDate,
//             request.EndDate);
//
//         var result = await _sender.Send(command, cancellationToken);
//
//         if (result.IsFailure) return BadRequest(result.Error);
//
//         return CreatedAtAction(nameof(GetBooking), new { id = result.Value }, result.Value);
//     }
// }
