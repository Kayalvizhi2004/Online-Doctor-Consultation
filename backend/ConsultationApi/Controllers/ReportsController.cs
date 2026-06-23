using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultationApi.Domain.Enums;

namespace ConsultationApi.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/reports/appointments-per-month
    [HttpGet("appointments-per-month")]
    public async Task<IActionResult> AppointmentsPerMonth()
    {
        var data = await _db.Appointments
            .GroupBy(a => new { Year = a.CreatedAt.Year, Month = a.CreatedAt.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        return Ok(ApiResponse<object>.SuccessResponse(data));
    }

    // GET /api/reports/doctors-by-specialization
    [HttpGet("doctors-by-specialization")]
    public async Task<IActionResult> DoctorsBySpecialization()
    {
        var data = await _db.DoctorProfiles
            .GroupBy(d => d.Specialization)
            .Select(g => new { Specialization = g.Key, Count = g.Count() })
            .ToListAsync();

        return Ok(ApiResponse<object>.SuccessResponse(data));
    }

    // GET /api/reports/most-active-doctors
    [HttpGet("most-active-doctors")]
    public async Task<IActionResult> MostActiveDoctors()
    {
        var data = await _db.Appointments
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.User)
            .GroupBy(a => a.DoctorId)
            .Select(g => new { DoctorId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        return Ok(ApiResponse<object>.SuccessResponse(data));
    }

    // GET /api/reports/most-active-patients
    [HttpGet("most-active-patients")]
    public async Task<IActionResult> MostActivePatients()
    {
        var data = await _db.Appointments
            .Include(a => a.Patient)
            .GroupBy(a => a.PatientId)
            .Select(g => new { PatientId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        return Ok(ApiResponse<object>.SuccessResponse(data));
    }

    // GET /api/reports/revenue-summary
    [HttpGet("revenue-summary")]
    public async Task<IActionResult> RevenueSummary()
    {
        var data = await _db.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.Status == AppointmentStatus.Completed)
            .GroupBy(a => new { Year = a.CreatedAt.Year, Month = a.CreatedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(a => (decimal?)(a.Doctor != null ? a.Doctor.ConsultationFee : 0)) ?? 0
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        return Ok(ApiResponse<object>.SuccessResponse(data));
    }

    // GET /api/reports/session-completion-rate
    [HttpGet("session-completion-rate")]
    public async Task<IActionResult> SessionCompletionRate()
    {
        var total = await _db.ConsultationSessions.CountAsync();
        var completed = await _db.ConsultationSessions.CountAsync(s => s.EndedAt != null);
        var rate = total == 0 ? 0 : (double)completed / total;
        return Ok(ApiResponse<object>.SuccessResponse(new { total, completed, rate }));
    }
}
