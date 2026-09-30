using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.DTOs.RoomService;
using ConferenceBooking.Models.Requests.Rooms;
using ConferenceBooking.Models.Response;

namespace ConferenceBooking.Mapping;

public static class RoomMapping
{
    public static CreateRoomDto ToDto(this CreateRoomRequest request)
    {
        return new CreateRoomDto
        {
            Name = request.RoomName,
            Capacity = request.Capacity,
            HourlyRate = request.HourlyRate,
            Services = request.Services
                .Select(service => service.ToDto())
                .ToList()
        };
    }

    public static CreateRoomServiceDto ToDto(
        this CreateRoomServiceRequest request)
    {
        return new CreateRoomServiceDto
        {
            AdditionalServiceId = request.AdditionalServiceId,
            Price = request.Price
        };
    }

    public static RoomResponse ToResponse(this RoomDto dto)
    {
        return new RoomResponse
        {
            Id = dto.Id,
            RoomName = dto.Name,
            Capacity = dto.Capacity,
            HourlyRate = dto.HourlyRate,
            Services = dto.RoomServices
                .Select(service => service.ToResponse())
                .ToList()
        };
    }

    public static RoomServiceResponse ToResponse(this RoomServiceDto dto)
    {
        return new RoomServiceResponse
        {
            AdditionalServiceId = dto.AdditionalServiceId,
            Price = dto.Price
        };
    }

    public static UpdateRoomDto ToDto(this UpdateRoomRequest request)
    {
        return new UpdateRoomDto
        {
            Name = request.RoomName,
            Capacity = request.Capacity,
            HourlyRate = request.HourlyRate,
            Services = request.Services
                .Select(service => service.ToDto())
                .ToList()
        };
    }
}