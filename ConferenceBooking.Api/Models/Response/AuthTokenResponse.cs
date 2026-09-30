namespace ConferenceBooking.Models.Response;

public class AuthTokenResponse
{
    public required string AccessToken { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}