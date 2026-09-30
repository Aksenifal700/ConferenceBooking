using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.DataAccess.Configurations;

public static class ServiceCollectionConfiguration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAdditionalServiceRepository, AdditionalServiceRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        return services;
    }
}