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

        builder.HasData(
            new Room
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
                RoomName = "Зал A",
                Capacity = 50,
                HourlyRate = 2000m,
                IsArchived = false
            },
            new Room
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"),
                RoomName = "Зал B",
                Capacity = 100,
                HourlyRate = 3500m,
                IsArchived = false
            },
            new Room
            {
                Id = Guid.Parse("cccccccc-cccc-4ccc-8ccc-cccccccccccc"),
                RoomName = "Зал C",
                Capacity = 30,
                HourlyRate = 1500m,
                IsArchived = false
            });
    }
}