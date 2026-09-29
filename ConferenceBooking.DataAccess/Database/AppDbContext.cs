using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.DataAccess.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Room> Rooms { get; set; }
    
    public DbSet<AdditionalService> AdditionalServices { get; set; }
    
    public DbSet<RoomService> RoomServices { get; set; }
    
    public DbSet<Booking> Bookings { get; set; }

    public DbSet<BookingService> BookingServices { get; set; }

    public DbSet<BookingPriceSegment> BookingPriceSegments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
} 