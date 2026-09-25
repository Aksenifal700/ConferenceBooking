using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.DataAccess.Database.EntityConfigurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(room => room.Id);

        builder.Property(room => room.RoomName)
            .IsRequired();
        
        builder.Property(room => room.Capacity)
            .IsRequired();
        
        builder.Property(room => room.IsArchived)
            .IsRequired();

        builder.Property(room => room.HourlyRate)
            .HasPrecision(18, 2);
    }
}