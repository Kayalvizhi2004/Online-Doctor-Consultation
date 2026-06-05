namespace ConsultationApi.Extensions;

public static class CorsExtension
{
    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AngularPolicy", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(
                        "http://localhost:4200");
            });
        });

        return services;
    }
}