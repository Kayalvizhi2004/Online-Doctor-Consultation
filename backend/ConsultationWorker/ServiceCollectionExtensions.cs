using ConsultationApi.Application;
using ConsultationApi.Infrastructure;
using ConsultationWorker.Configurations;
using ConsultationWorker.RabbitMQ;
using ConsultationWorker.Services;
using ConsultationWorker.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultationWorker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Reuse the application's DI registrations
        services.AddInfrastructure(configuration); 
        services.AddApplication();

        // Bind worker configuration
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));

        // Worker specific services
        services.AddSingleton<RabbitMqConnectionFactory>();
        services.AddSingleton<NotificationProcessor>();
        services.AddSingleton<EmailSimulator>();

        // Hosted consumer
        services.AddHostedService<AppointmentEventConsumer>();

        return services;
    }
}
