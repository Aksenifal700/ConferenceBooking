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
        
        builder.HasData(
            new AdditionalService
            {
                Id = Guid.Parse("11111111-1111-4111-8111-111111111111"),
                Name = "Проєктор"
            },
            new AdditionalService
            {
                Id = Guid.Parse("22222222-2222-4222-8222-222222222222"),
                Name = "Wi-Fi"
            },
            new AdditionalService
            {
                Id = Guid.Parse("33333333-3333-4333-8333-333333333333"),
                Name = "Звук"
            });
    }
}