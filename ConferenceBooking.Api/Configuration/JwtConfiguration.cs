using System.Net;
using System.Security.Claims;
using System.Text;
using ConferenceBooking.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ConferenceBooking.Configuration;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var secret = configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(issuer)
            || string.IsNullOrWhiteSpace(audience)
            || string.IsNullOrWhiteSpace(secret)
            || Encoding.UTF8.GetByteCount(secret) < 32
            || !int.TryParse(configuration["Jwt:ExpirationMinutes"], out var minutes)
            || minutes <= 0 || minutes > 1440)
        {
            throw new InvalidOperationException(
                "Configure Jwt:Issuer, Jwt:Audience, a random SecretKey of at least 32 bytes, and ExpirationMinutes between 1 and 1440.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secret)),
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                    NameClaimType = ClaimTypes.NameIdentifier,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var id = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (!Guid.TryParse(id, out var userId) || userId == Guid.Empty)
                            context.Fail("The token must contain a valid user identifier.");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.Headers["WWW-Authenticate"] = "Bearer";
                        return context.Response.WriteAsJsonAsync(
                            new ExceptionResponse(HttpStatusCode.Unauthorized, "Authentication is required."));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return context.Response.WriteAsJsonAsync(
                            new ExceptionResponse(HttpStatusCode.Forbidden, "Access is denied."));
                    }
                };
            });
        services.AddAuthorization();
        return services;
    }
}
