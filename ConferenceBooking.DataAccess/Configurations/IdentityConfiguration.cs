using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.DataAccess.Database;
using ConferenceBooking.DataAccess.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.DataAccess.Configurations;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 10;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.AllowedForNewUsers = false;
        })
        .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}