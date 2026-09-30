using ConferenceBooking.Application.DTOs.Reports;
using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Models.Requests.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("revenue")]
    public async Task<ActionResult<List<RoomRevenueDto>>> GetRevenue(
        [FromQuery] ReportRequest request, CancellationToken cancellationToken)
        => Ok(await reportService.GetRevenueAsync(request.From, request.To, cancellationToken));

    [HttpGet("occupancy")]
    public async Task<ActionResult<List<RoomOccupancyDto>>> GetOccupancy(
        [FromQuery] ReportRequest request, CancellationToken cancellationToken)
        => Ok(await reportService.GetOccupancyAsync(request.From, request.To, cancellationToken));

    [HttpGet("services")]
    public async Task<ActionResult<List<ServicePopularityDto>>> GetServices(
        [FromQuery] ReportRequest request, CancellationToken cancellationToken)
        => Ok(await reportService.GetServicesAsync(request.From, request.To, cancellationToken));
}
