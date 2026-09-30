namespace ConferenceBooking.Application.DTOs.Auth;

public class AuthTokenDto
{
    public required string AccessToken { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}