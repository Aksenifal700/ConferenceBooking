using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.Application.Configuration;

public static class ServiceCollectionConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
        
        return services;
    }
}