using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.DataAccess.Database.EntityConfigurations;

public class BookingServiceConfiguration : IEntityTypeConfiguration<BookingService>
{
    public void Configure(EntityTypeBuilder<BookingService> builder)
    {
        builder.ToTable("BookingServices", table =>
        {
            table.HasCheckConstraint(
                "CK_BookingServices_Price",
                "\"Price\" >= 0");
        });

        builder.HasKey(service => new
        {
            service.BookingId,
            service.AdditionalServiceId
        });

        builder.Property(service => service.BookingId)
            .IsRequired();

        builder.Property(service => service.AdditionalServiceId)
            .IsRequired();

        builder.Property(service => service.Name)
            .IsRequired();

        builder.Property(service => service.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne<Booking>()
            .WithMany(booking => booking.Services)
            .HasForeignKey(service => service.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AdditionalService>()
            .WithMany()
            .HasForeignKey(service => service.AdditionalServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}