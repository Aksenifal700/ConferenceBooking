using System.Security.Claims;
using System.Security.Cryptography;
using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Mapping;
using ConferenceBooking.Models.Requests.Booking;
using ConferenceBooking.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Controllers;

[ApiController]
[Authorize]
[Route("api/bookings")]
public class BookingController : ControllerBase 
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            return Unauthorized();
        }
        
        var booking = await _bookingService.CreateAsync(
            request.ToDto(),
            userId,
            cancellationToken);
        
        return StatusCode(StatusCodes.Status201Created,
            booking.ToResponse());
    }
    
}