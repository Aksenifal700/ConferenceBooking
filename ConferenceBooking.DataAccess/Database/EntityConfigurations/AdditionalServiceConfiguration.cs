using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.DataAccess.Database.EntityConfigurations;

public class AdditionalServiceConfiguration : IEntityTypeConfiguration<AdditionalService>
{
    public void Configure(EntityTypeBuilder<AdditionalService> builder)
    {
        builder.ToTable("AdditionalServices");

        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name)
            .IsRequired();
    }
}