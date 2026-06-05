using System.Net;
using System.Text.Json;
using ConsultationApi.Application.DTOs.Common;

namespace ConsultationApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var status = ex switch
            {
                ConsultationApi.Application.Exceptions.NotFoundException => (int)HttpStatusCode.NotFound,
                ConsultationApi.Application.Exceptions.ConflictException => (int)HttpStatusCode.Conflict,
                ConsultationApi.Application.Exceptions.ForbiddenException => (int)HttpStatusCode.Forbidden,
                ConsultationApi.Application.Exceptions.BusinessRuleViolationException => 422,
                ConsultationApi.Application.Exceptions.ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var problem = new
            {
                type = "about:blank",
                title = ex.Message,
                status = status,
                detail = _env.IsDevelopment() ? ex.ToString() : ex.Message,
                traceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;

            var json = JsonSerializer.Serialize(problem);

            await context.Response.WriteAsync(json);
        }
    }
}