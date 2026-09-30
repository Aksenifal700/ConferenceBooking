using ConferenceBooking.Application.DTOs.Auth;

namespace ConferenceBooking.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    AuthTokenDto GenerateToken(TokenGenerationDto dto);
}
