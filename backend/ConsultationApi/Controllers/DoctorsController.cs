using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Doctors;
using ConsultationApi.Application.Interfaces.Doctors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConsultationApi.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(
        IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;

        var sub = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub))
            sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(sub))
            return false;

        return Guid.TryParse(sub, out userId);
    }

    // GET: api/doctors
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<DoctorListDto>>), 200)]
    public async Task<IActionResult> GetDoctors(
        [FromQuery] string? specialization,
        [FromQuery] DateOnly? availableDate)
    {
        var filter = new DoctorFilterDto
        {
            Specialization = specialization,
            AvailableDate = availableDate
        };

        var result =
            await _doctorService.GetDoctorsAsync(filter);

        return Ok(result);
    }

    // GET: api/doctors/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ApiResponse<DoctorDetailDto>), 200)]
    public async Task<IActionResult> GetDoctorById(
        Guid id)
    {
        var result =
            await _doctorService.GetDoctorByIdAsync(id);

        return Ok(result);
    }

    // PUT: api/doctors/profile
    [HttpPut("profile")]
    [Authorize(Policy = "RequireDoctor")]
    [ProducesResponseType(
        typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] DoctorProfileUpdateDto request)
    {
        if (!TryGetUserId(out var doctorId))
            return Unauthorized(ApiResponse<string>.Failure("Unauthorized", 401));

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var msg = string.Join("; ", errors);
            return BadRequest(ApiResponse<string>.Failure(msg, 400));
        }

        var result = await _doctorService.UpdateProfileAsync(doctorId, request);

        return Ok(result);
    }

    // POST: api/doctors/slots
    [HttpPost("slots")]
    [Authorize(Policy = "RequireDoctor")]
    [ProducesResponseType(
        typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> AddSlot(
        [FromBody] CreateSlotDto request)
    {
        if (!TryGetUserId(out var doctorId))
            return Unauthorized(ApiResponse<string>.Failure("Unauthorized", 401));

        var result = await _doctorService.AddSlotAsync(doctorId, request);

        return Ok(result);
    }

    // DELETE: api/doctors/slots/{slotId}
    [HttpDelete("slots/{slotId:guid}")]
    [Authorize(Policy = "RequireDoctor")]
    [ProducesResponseType(
        typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> DeleteSlot(
        Guid slotId)
    {
        if (!TryGetUserId(out var doctorId))
            return Unauthorized(ApiResponse<string>.Failure("Unauthorized", 401));
        var result = await _doctorService.RemoveSlotAsync(doctorId, slotId);

        return Ok(result);
    }

    // PATCH: api/doctors/availability
    [HttpPatch("availability")]
    [Authorize(Policy = "RequireDoctor")]
    [ProducesResponseType(
        typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> ToggleAvailability(
        [FromBody]
        ToggleAvailabilityDto request)
    {
        if (!TryGetUserId(out var doctorId))
            return Unauthorized(ApiResponse<string>.Failure("Unauthorized", 401));

        var result = await _doctorService.ToggleAvailabilityAsync(doctorId, request.IsAvailable);

        return Ok(result);
    }

    // GET: api/doctors/availability
    [HttpGet("availability")]
    [Authorize(Policy = "RequireDoctor")]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<AvailabilitySlotDto>>), 200)]
    public async Task<IActionResult> GetMyAvailability()
    {
        if (!TryGetUserId(out var doctorId))
            return Unauthorized(ApiResponse<string>.Failure("Unauthorized", 401));

        var result = await _doctorService.GetDoctorByIdAsync(doctorId);

        if (result == null || result.Data == null)
            return Ok(ApiResponse<IEnumerable<AvailabilitySlotDto>>.SuccessResponse(Array.Empty<AvailabilitySlotDto>()));

        return Ok(ApiResponse<IEnumerable<AvailabilitySlotDto>>.SuccessResponse(result.Data.Slots));
    }
}