using ConsultationApi.Middleware;

namespace ConsultationApi.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseCustomMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }
}