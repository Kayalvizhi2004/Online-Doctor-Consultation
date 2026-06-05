using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConsultationApi.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtKey =
            configuration["Jwt:Secret"] ?? configuration["Jwt:SecretKey"] ?? configuration.GetValue<string>("Jwt:SecretKey");

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT secret is not configured. Set 'Jwt:Secret' or 'Jwt:SecretKey' in configuration or environment.");

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/consultation"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        try
                        {
                            var loggerFactory = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
                            var logger = loggerFactory?.CreateLogger("JwtAuth") ?? Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger("JwtAuth");
                            logger.LogError(context.Exception, "JWT authentication failed: {Message}", context.Exception.Message);
                        }
                        catch { }

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        try
                        {
                            var loggerFactory = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
                            var logger = loggerFactory?.CreateLogger("JwtAuth") ?? Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger("JwtAuth");
                            logger.LogInformation("JWT validated for {Sub}", context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value);
                        }
                        catch { }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        // Role-based policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdmin", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("RequireDoctor", policy =>
                policy.RequireRole("Doctor"));

            options.AddPolicy("RequirePatient", policy =>
                policy.RequireRole("Patient"));
        });

        return services;
    }
}