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

        builder.HasOne<AdditionalService>()
            .WithMany()
            .HasForeignKey(service => service.AdditionalServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}