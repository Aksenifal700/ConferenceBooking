namespace ConferenceBooking.Application.DTOs.Auth;

public class TokenGenerationDto
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
}