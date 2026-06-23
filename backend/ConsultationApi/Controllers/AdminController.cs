using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Infrastructure.Data;
using ConsultationApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/admin/kpis
    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis()
    {
        var totalUsers = await _db.Users.CountAsync();
        var totalDoctors = await _db.DoctorProfiles.CountAsync();
        var totalPatients = await _db.Users.CountAsync(u => u.Role == UserRole.Patient);
        var totalAppointments = await _db.Appointments.CountAsync();
        var activeSessions = await _db.ConsultationSessions.CountAsync(s => s.EndedAt == null);
        var completedSessions = await _db.ConsultationSessions.CountAsync(s => s.EndedAt != null);

        var avgDoctorRating = await _db.Reviews
            .Select(r => (double?)r.Rating)
            .AverageAsync() ?? 0.0;

        var pendingAppointments = await _db.Appointments.CountAsync(a => a.Status == ConsultationApi.Domain.Enums.AppointmentStatus.Pending);

        var result = new
        {
            totalUsers,
            totalDoctors,
            totalPatients,
            totalAppointments,
            activeSessions,
            completedSessions,
            avgDoctorRating,
            pendingAppointments
        };

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    // GET /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(s) || u.Email.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var roleEnum))
            query = query.Where(u => u.Role == roleEnum);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                role = u.Role.ToString(),
                phone = u.Phone,
                created = u.CreatedAt
            })
            .ToListAsync();

        var paged = new PagedResponse<object> { Items = items, Page = pageNumber, PageSize = pageSize, TotalCount = total };

        return Ok(ApiResponse<PagedResponse<object>>.SuccessResponse(paged));
    }

    // GET /api/admin/users/{id}
    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _db.Users.Include(u => u.DoctorProfile).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound(ApiResponse<string>.Failure("User not found", 404));

        return Ok(ApiResponse<object>.SuccessResponse(new {
            id = user.Id,
            fullName = user.FullName,
            email = user.Email,
            role = user.Role.ToString(),
            phone = user.Phone,
            created = user.CreatedAt,
            doctorProfile = user.DoctorProfile
        }));
    }

    // PUT /api/admin/users/{id}
    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] dynamic body)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(ApiResponse<string>.Failure("User not found", 404));
        
        // superficial updates: fullName, phone, role
        try
        {
            if (body.fullName != null) user.FullName = (string)body.fullName;
            if (body.phone != null) user.Phone = (string)body.phone;
            if (body.role != null)
            {
                var roleStr = (string)body.role;
                if (Enum.TryParse<UserRole>(roleStr, true, out var rr))
                    user.Role = rr;
            }

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<string>.SuccessResponse("User updated"));
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Failure(ex.Message, 500));
        }
    }

    // PATCH activate
    [HttpPatch("users/{id:guid}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(ApiResponse<string>.Failure("User not found", 404));
        user.UpdatedAt = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.SuccessResponse("User activated"));
    }

    // PATCH deactivate
    [HttpPatch("users/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(ApiResponse<string>.Failure("User not found", 404));
        user.UpdatedAt = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.SuccessResponse("User deactivated"));
    }

    // DELETE /api/admin/users/{id}
    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(ApiResponse<string>.Failure("User not found", 404));
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.SuccessResponse("User deleted"));
    }

    // Patients endpoints reuse users where role == Patient
    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.Users.Where(u => u.Role == UserRole.Patient);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new { id = u.Id, name = u.FullName, email = u.Email, phone = u.Phone, created = u.CreatedAt })
            .ToListAsync();

        var paged = new PagedResponse<object> { Items = items, Page = pageNumber, PageSize = pageSize, TotalCount = total };
        return Ok(ApiResponse<PagedResponse<object>>.SuccessResponse(paged));
    }

    [HttpPut("patients/{id:guid}")]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] dynamic body)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null || user.Role != UserRole.Patient) return NotFound(ApiResponse<string>.Failure("Patient not found", 404));
        if (body.fullName != null) user.FullName = (string)body.fullName;
        if (body.phone != null) user.Phone = (string)body.phone;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.SuccessResponse("Patient updated"));
    }

    [HttpPatch("patients/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivatePatient(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null || user.Role != UserRole.Patient) return NotFound(ApiResponse<string>.Failure("Patient not found", 404));
        user.UpdatedAt = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.SuccessResponse("Patient deactivated"));
    }

    [HttpDelete("patients/{id:guid}")]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null || user.Role != UserRole.Patient) return NotFound(ApiResponse<string>.Failure("Patient not found", 404));
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.SuccessResponse("Patient deleted"));
    }
}
