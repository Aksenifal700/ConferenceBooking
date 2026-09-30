using ConferenceBooking.Application.DTOs.Auth;

namespace ConferenceBooking.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Guid> RegisterAsync(RegisterDto dto);
    Task<AuthTokenDto> LoginAsync(LoginDto dto);
}