using ConsultationApi.Application;
using ConsultationApi.Infrastructure;
using ConsultationWorker.Configurations;
using ConsultationWorker.RabbitMQ;
using ConsultationWorker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultationWorker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bring in shared application/infrastructure registrations
        services.AddInfrastructure(configuration);
        services.AddApplication();

        // Worker-specific configuration
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));

        // Worker services
        services.AddSingleton<RabbitMqConnectionFactory>();
        services.AddSingleton<NotificationProcessor>();
        services.AddSingleton<EmailSimulator>();

        // Hosted background consumer
        services.AddHostedService<AppointmentEventConsumer>();

        return services;
    }
}
