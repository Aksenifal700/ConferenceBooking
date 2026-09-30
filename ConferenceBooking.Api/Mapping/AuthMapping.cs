using ConferenceBooking.Application.DTOs.Auth;
using ConferenceBooking.Models.Requests.Auth;
using ConferenceBooking.Models.Response;

namespace ConferenceBooking.Mapping;

public static class AuthMapping
{
    public static RegisterDto ToDto(this RegisterRequest request)
    {
        return new RegisterDto
        {
            Email = request.Email,
            Password = request.Password
        };
    }

    public static LoginDto ToDto(this LoginRequest request)
    {
        return new LoginDto
        {
            Email = request.Email,
            Password = request.Password
        };
    }

    public static AuthTokenResponse ToResponse(this AuthTokenDto dto)
    {
        return new AuthTokenResponse
        {
            AccessToken = dto.AccessToken,
            ExpiresAt = dto.ExpiresAt
        };
    }
}