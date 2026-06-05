using ConsultationApi.Application.DTOs.Reviews;
using ConsultationApi.Application.Interfaces.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApi.Controllers;

[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(
        IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    //----------------------------------------------------
    // POST review
    // POST /api/appointments/{id}/review
    //----------------------------------------------------

    [Authorize(Roles = "Patient")]
    [HttpPost("api/appointments/{id:guid}/review")]
    public async Task<IActionResult> SubmitReview(
        Guid id,
        [FromBody] CreateReviewDto dto)
    {
        var patientId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _reviewService
                .AddReviewAsync(patientId, id, dto);

        return StatusCode(
            response.StatusCode,
            response);
    }

    //----------------------------------------------------
    // GET doctor reviews
    // GET /api/doctors/{id}/reviews
    //----------------------------------------------------

    [AllowAnonymous]
    [HttpGet("api/doctors/{id:guid}/reviews")]
    public async Task<IActionResult> GetDoctorReviews(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response =
            await _reviewService
                .GetDoctorReviewsAsync(
                    id,
                    pageNumber,
                    pageSize);

        return StatusCode(
            response.StatusCode,
            response);
    }
}