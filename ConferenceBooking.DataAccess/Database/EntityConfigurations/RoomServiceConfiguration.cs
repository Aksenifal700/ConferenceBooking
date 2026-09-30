using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.DataAccess.Database.EntityConfigurations;

public class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
{
    public void Configure(EntityTypeBuilder<RoomService> builder)
    {
        builder.ToTable("RoomServices");

        builder.HasKey(service => new
        {
            service.RoomId,
            service.AdditionalServiceId
        });

        builder.Property(service => service.RoomId)
            .IsRequired();

        builder.Property(service => service.AdditionalServiceId)
            .IsRequired();

        builder.Property(service => service.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne<Room>()
            .WithMany(room => room.Services)
            .HasForeignKey(service => service.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(service => service.AdditionalService)
            .WithMany()
            .HasForeignKey(service => service.AdditionalServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        var roomIds = new[]
        {
            Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
            Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"),
            Guid.Parse("cccccccc-cccc-4ccc-8ccc-cccccccccccc")
        };

        foreach (var roomId in roomIds)
        {
            builder.HasData(
                new RoomService
                {
                    RoomId = roomId,
                    AdditionalServiceId =
                        Guid.Parse("11111111-1111-4111-8111-111111111111"),
                    Price = 500m
                },
                new RoomService
                {
                    RoomId = roomId,
                    AdditionalServiceId =
                        Guid.Parse("22222222-2222-4222-8222-222222222222"),
                    Price = 300m
                },
                new RoomService
                {
                    RoomId = roomId,
                    AdditionalServiceId =
                        Guid.Parse("33333333-3333-4333-8333-333333333333"),
                    Price = 700m
                });
        }
    }
}