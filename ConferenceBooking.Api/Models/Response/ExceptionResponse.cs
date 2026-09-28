using System.Net;

namespace ConferenceBooking.Models.Response;

public record ExceptionResponse(HttpStatusCode StatusCode, string Message);
