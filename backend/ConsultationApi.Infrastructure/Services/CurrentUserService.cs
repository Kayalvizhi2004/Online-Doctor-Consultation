using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ConsultationApi.Domain.Interfaces.Services;

namespace ConsultationApi.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor
        _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor
            httpContextAccessor)
    {
        _httpContextAccessor =
            httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value =
                _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return value == null
                ? Guid.Empty
                : Guid.Parse(value);
        }
    }

    public string Email
        => _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirstValue(
                ClaimTypes.Email)
            ?? string.Empty;

    public string Role
        => _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirstValue(
                ClaimTypes.Role)
            ?? string.Empty;
}