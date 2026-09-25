using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Domain.Entities;
using RoomServiceEntity = ConferenceBooking.Domain.Entities.RoomService;

namespace ConferenceBooking.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IAdditionalServiceRepository _additionalServiceRepository;

    public RoomService(IRoomRepository roomRepository, IAdditionalServiceRepository additionalServiceRepository)
    {
        _roomRepository = roomRepository;
        _additionalServiceRepository = additionalServiceRepository;
    }

    public async Task<Guid> CreateRoomAsync(CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var serviceIds = dto.Services
            .Select(service => service.AdditionalServiceId)
            .ToArray();

        if (serviceIds.Length > 0)
        {
            var existingIds = await _additionalServiceRepository.GetExistingIdsAsync(
                serviceIds,
                cancellationToken);

            var missingIds = serviceIds
                .Except(existingIds)
                .ToArray();

            if (missingIds.Length > 0)
            {
                throw new NotFoundException($"Additional services not found: {string.Join(", ", missingIds)}");
            }
        }
        
        var roomId = Guid.NewGuid();

        var room = new Room
        {
            Id = roomId,
            RoomName = dto.Name.Trim(),
            Capacity = dto.Capacity,
            HourlyRate = dto.HourlyRate,
            IsArchived = false,
            Services = dto.Services
                .Select(service => new RoomServiceEntity
                {
                    RoomId = roomId,
                    AdditionalServiceId = service.AdditionalServiceId,
                    Price = service.Price
                })
                .ToList()
        };
        
        await _roomRepository.AddAsync(room, cancellationToken);

        return room.Id;
    }
}