using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.DataAccess.Database.EntityConfigurations;

public class BookingPriceSegmentConfiguration
    : IEntityTypeConfiguration<BookingPriceSegment>
{
    public void Configure(EntityTypeBuilder<BookingPriceSegment> builder)
    {
        builder.ToTable("BookingPriceSegments", table =>
        {
            table.HasCheckConstraint(
                "CK_BookingPriceSegments_TimeRange",
                "\"EndsAt\" > \"StartsAt\"");
        });

        builder.HasKey(segment => segment.Id);

        builder.Property(segment => segment.Id)
            .IsRequired();

        builder.Property(segment => segment.BookingId)
            .IsRequired();

        builder.Property(segment => segment.StartsAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(segment => segment.EndsAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(segment => segment.BaseHourlyRate)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(segment => segment.Multiplier)
            .HasPrecision(4, 2)
            .IsRequired();

        builder.Property(segment => segment.Amount)
            .HasPrecision(28, 10)
            .IsRequired();

        builder.HasOne<Booking>()
            .WithMany(booking => booking.PriceSegments)
            .HasForeignKey(segment => segment.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}