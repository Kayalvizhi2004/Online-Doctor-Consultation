using ConsultationApi.Application.DTOs.Auth;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.Interfaces.Auth;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Enums;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Domain.Interfaces.Authentication;
using Microsoft.Extensions.Configuration;

namespace ConsultationApi.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IPasswordHasher _hasher;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public AuthService(
        IUserRepository users,
        IJwtTokenGenerator jwt,
        IPasswordHasher hasher,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _users = users;
        _jwt = jwt;
        _hasher = hasher;
        _configuration = configuration;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(
        RegisterRequestDto request)
    {
        var existing =
            await _users.GetByEmailAsync(
                request.Email);

        if (existing != null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
                PasswordHash =
                _hasher.Hash(
                    request.Password),
            Role = Enum.Parse<UserRole>(request.Role, true),
            CreatedAt = DateTime.UtcNow
        };

        // If registering a doctor, create an associated DoctorProfile
        if (user.Role == UserRole.Doctor)
        {
            user.DoctorProfile = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Specialization = request.Specialization ?? string.Empty,
                Bio = string.Empty,
                ConsultationFee = 0m,
                IsAvailable = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        // Do NOT issue access or refresh tokens on registration.
        // Tokens are issued only during explicit login.
        return new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Registered successfully",
            Data = null
        };
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(
        LoginRequestDto request)
    {
        var user =
            await _users.GetByEmailAsync(
                request.Email);

        if (user == null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Invalid credentials"
            };
        }

        var valid =
            _hasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!valid)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Invalid credentials"
            };
        }

        var token = _jwt.GenerateToken(user);

        // create and persist refresh token
        var refreshToken = Guid.NewGuid().ToString();
        var refreshDays = _configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);

        var rt = new ConsultationApi.Domain.Entities.Users.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddRefreshTokenAsync(rt);
        await _users.SaveChangesAsync();

        var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 60);

        return new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Login successful",
            Data = new AuthResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            }
        };
    }

    public async Task<ApiResponse<AuthResponseDto>>
        RefreshTokenAsync(
            string refreshToken)
    {
        var existing = await _users.GetRefreshTokenAsync(refreshToken);

        if (existing == null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Invalid refresh token"
            };
        }

        if (existing.IsRevoked || existing.ExpiresAt < DateTime.UtcNow)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Refresh token expired or revoked"
            };
        }

        // Revoke existing
        existing.IsRevoked = true;
        existing.RevokedAt = DateTime.UtcNow;

        // load user
        var user = await _users.GetByIdAsync(existing.UserId);
        if (user == null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "User not found for refresh token"
            };
        }

        // create new refresh token
        var newRefresh = Guid.NewGuid().ToString();
        var refreshDays = _configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);

        var rt = new ConsultationApi.Domain.Entities.Users.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefresh,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddRefreshTokenAsync(rt);
        await _users.SaveChangesAsync();

        var access = _jwt.GenerateToken(user);
        var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 60);

        return new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Token refreshed",
            Data = new AuthResponseDto
            {
                AccessToken = access,
                RefreshToken = newRefresh,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            }
        };
    }

    public async Task<ApiResponse<UserProfileDto>>
        GetCurrentUserAsync(Guid userId)
    {
        var user =
            await _users.GetByIdAsync(
                userId);

        if (user == null)
        {
            return new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            };
        }

        return new ApiResponse<UserProfileDto>
        {
            Success = true,
            Data = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString()
            }
        };
    }

    public async Task<ApiResponse<string>> UpdateCurrentUserAsync(Guid userId, UpdateUserProfileDto dto)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found",
                StatusCode = 404
            };
        }

        user.FullName = dto.FullName ?? user.FullName;
        user.Phone = dto.Phone ?? user.Phone;
        // Optional fields - stored as free text on User entity for now
        // Save changes
        await _users.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Profile updated"
        };
    }
}