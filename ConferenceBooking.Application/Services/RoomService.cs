using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Application.Mapping;
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

        await ValidateServiceIdsAsync(serviceIds, cancellationToken);

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

    public async Task<RoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            throw new NotFoundException(
                $"Room with ID '{id}' was not found");
        }

        return room.ToDto();
    }

    public async Task UpdateAsync(Guid id, UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var serviceIds = dto.Services
            .Select(service => service.AdditionalServiceId)
            .ToArray();
        
        await ValidateServiceIdsAsync(serviceIds, cancellationToken);

        var updated = await _roomRepository.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (!updated)
        {
            throw new NotFoundException(
                $"Room with ID '{id}' was not found.");
        }
    }

    public async Task<IReadOnlyList<RoomDto>> GetAvailableAsync(DateTimeOffset startsAt, DateTimeOffset endsAt, int capacity,
        CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.GetAvailableAsync(
            startsAt, 
            endsAt,
            capacity, 
            cancellationToken);
        
        return rooms
            .Select(room => room.ToDto())
            .ToList();
    }

    private async Task ValidateServiceIdsAsync(
        IReadOnlyCollection<Guid> serviceIds,
        CancellationToken cancellationToken)
    {
        if (serviceIds.Count == 0)
        {
            return;
        }

        var existingIds = await _additionalServiceRepository.GetExistingIdsAsync(
            serviceIds,
            cancellationToken);

        var missingIds = serviceIds
            .Except(existingIds)
            .ToArray();

        if (missingIds.Length > 0)
        {
            throw new NotFoundException(
                $"Additional services not found: {string.Join(", ", missingIds)}");
        }
    }
    
}