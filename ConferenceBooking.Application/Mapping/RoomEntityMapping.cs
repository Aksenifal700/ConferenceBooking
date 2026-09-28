using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.DTOs.RoomService;
using ConferenceBooking.Domain.Entities;
using RoomServiceEntity = ConferenceBooking.Domain.Entities.RoomService;

namespace ConferenceBooking.Application.Mapping;

public static class RoomEntityMapping
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.RoomName,
            Capacity = room.Capacity,
            HourlyRate = room.HourlyRate,
            RoomServices = room.Services
                .Select(service => service.ToDto())
                .ToList()
        };
    }

    public static RoomServiceDto ToDto(this RoomServiceEntity service)
    {
        return new RoomServiceDto
        {
            AdditionalServiceId = service.AdditionalServiceId,
            Price = service.Price
        };
    }
}
