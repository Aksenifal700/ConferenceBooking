using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Application.Services;
using ConferenceBooking.Domain.Pricing;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.Application.Configuration;

public static class ServiceCollectionConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<BookingPriceCalculator>();

        return services;
    }
}