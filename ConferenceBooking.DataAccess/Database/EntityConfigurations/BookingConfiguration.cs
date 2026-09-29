using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.DataAccess.Database.EntityConfigurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings", table =>
        {
            table.HasCheckConstraint(
                "CK_Bookings_TimeRange",
                "\"EndsAt\" > \"StartsAt\"");

            table.HasCheckConstraint(
                "CK_Bookings_TotalPrice",
                "\"TotalPrice\" >= 0");
        });

        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.Id)
            .IsRequired();

        builder.Property(booking => booking.RoomId)
            .IsRequired();

        builder.Property(booking => booking.UserId)
            .IsRequired();

        builder.Property(booking => booking.StartsAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(booking => booking.EndsAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(booking => booking.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(booking => booking.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(booking => booking.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(booking => booking.UserId);
    }
}