using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Database;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.DataAccess.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        _context.Rooms.Add(room);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .AsNoTracking()
            .Include(room => room.Services)
            .FirstOrDefaultAsync(room => room.Id == id && !room.IsArchived,
                cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .Include(room => room.Services)
            .FirstOrDefaultAsync(
                room => room.Id == id && !room.IsArchived,
                cancellationToken);

        if (room is null)
        {
            return false;
        }

        room.RoomName = dto.Name.Trim();
        room.Capacity = dto.Capacity;
        room.HourlyRate = dto.HourlyRate;

        var requestedServices = dto.Services.ToDictionary(
            service => service.AdditionalServiceId);

        foreach (var existingService in room.Services.ToList())
        {
            if (requestedServices.TryGetValue(
                    existingService.AdditionalServiceId,
                    out var requestedService))
            {
                existingService.Price = requestedService.Price;

                requestedServices.Remove(
                    existingService.AdditionalServiceId);
            }
            else
            {
                room.Services.Remove(existingService);
                _context.RoomServices.Remove(existingService);
            }
        }

        foreach (var newService in requestedServices.Values)
        {
            room.Services.Add(new RoomService
            {
                RoomId = room.Id,
                AdditionalServiceId = newService.AdditionalServiceId,
                Price = newService.Price
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}