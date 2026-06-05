using ConsultationApi.Application;
using ConsultationApi.Infrastructure;

namespace ConsultationApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddControllers();

        return services;
    }
}