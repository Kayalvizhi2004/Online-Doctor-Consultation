using System.Text.Json;

namespace ConsultationApi.Extensions;

public static class SignalRExtension
{
    public static IServiceCollection AddSignalRServices(
        this IServiceCollection services)
    {
        services
            .AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions
                    .PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase;
            });

        return services;
    }
}