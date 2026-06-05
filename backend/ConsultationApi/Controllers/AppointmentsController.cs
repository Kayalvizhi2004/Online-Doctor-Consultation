using ConsultationApi.Application.DTOs.Appointments;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.Interfaces.Appointments;
using ConsultationApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApi.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(
        IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    //---------------------------------------------------
    // POST /api/appointments
    // Patient books appointment
    //---------------------------------------------------

    [Authorize(Roles = "Patient")]
    [HttpPost]
    public async Task<IActionResult> BookAppointment(
        [FromBody] BookAppointmentDto dto)
    {
        var patientId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _appointmentService.BookAppointmentAsync(patientId, dto);

        return StatusCode(response.StatusCode, response);
    }

    //---------------------------------------------------
    // GET /api/appointments
    // Own appointments
    //---------------------------------------------------

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] AppointmentStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ??
            User.FindFirst("role")?.Value ?? string.Empty;

        var filter = new AppointmentFilterDto
        {
            Status = status?.ToString(),
            Page = pageNumber,
            PageSize = pageSize
        };

        var response =
            await _appointmentService.GetAppointmentsAsync(userId, role, filter);

        return StatusCode(response.StatusCode, response);
    }

    //---------------------------------------------------
    // GET /api/appointments/{id}
    //---------------------------------------------------

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAppointmentById(
        Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _appointmentService.GetAppointmentAsync(id, userId);

        return StatusCode(response.StatusCode, response);
    }

    //---------------------------------------------------
    // PATCH confirm
    // Doctor confirms appointment
    //---------------------------------------------------

    [Authorize(Roles = "Doctor")]
    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(
        Guid id)
    {
        var doctorId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _appointmentService
                .ConfirmAppointmentAsync(doctorId, id);

        return StatusCode(response.StatusCode, response);
    }

    //---------------------------------------------------
    // PATCH cancel
    // Patient or Doctor
    //---------------------------------------------------

    [Authorize]
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(
        Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _appointmentService
                .CancelAppointmentAsync(userId, id);

        return StatusCode(response.StatusCode, response);
    }

    //---------------------------------------------------
    // POST session/start
    //---------------------------------------------------

    [Authorize(Roles = "Doctor")]
    [HttpPost("{id:guid}/session/start")]
    public async Task<IActionResult> StartSession(
        Guid id)
    {
        var doctorId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _appointmentService
                .StartSessionAsync(doctorId, id);

        return StatusCode(response.StatusCode, response);
    }

    //---------------------------------------------------
    // POST session/end
    //---------------------------------------------------

    [Authorize(Roles = "Doctor")]
    [HttpPost("{id:guid}/session/end")]
    public async Task<IActionResult> EndSession(
        Guid id)
    {
        var doctorId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _appointmentService
                .EndSessionAsync(doctorId, id);

        return StatusCode(response.StatusCode, response);
    }
}