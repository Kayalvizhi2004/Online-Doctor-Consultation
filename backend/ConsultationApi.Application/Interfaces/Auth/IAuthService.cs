using ConsultationApi.Application.DTOs.Auth;
using ConsultationApi.Application.DTOs.Common;

namespace ConsultationApi.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(
        RegisterRequestDto request);

    Task<ApiResponse<AuthResponseDto>> LoginAsync(
        LoginRequestDto request);

    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(
        string refreshToken);

    Task<ApiResponse<UserProfileDto>> GetCurrentUserAsync(
        Guid userId);
}