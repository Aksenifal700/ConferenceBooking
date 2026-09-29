using ConferenceBooking.Application.DTOs.Bookings;
using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Mapping;

public static class BookingEntityMapping
{
    public static BookingDto ToDto(this Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            StartsAt = booking.StartsAt,
            EndsAt = booking.EndsAt,
            TotalPrice = booking.TotalPrice
        };
    }
}