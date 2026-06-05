using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Interfaces.Authentication;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConsultationApi.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

        public string GenerateToken(User user)
    {
        var key = _configuration["Jwt:Secret"] ?? _configuration["Jwt:SecretKey"] ?? _configuration.GetValue<string>("Jwt:SecretKey");

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT secret is not configured. Set 'Jwt:Secret' or 'Jwt:SecretKey' in configuration or environment.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 15);

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}