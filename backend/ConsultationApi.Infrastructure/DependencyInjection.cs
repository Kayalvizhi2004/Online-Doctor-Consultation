using ConsultationApi.Infrastructure.Authentication;
using ConsultationApi.Infrastructure.Data;
using ConsultationApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConsultationApi.Infrastructure.Redis;
using ConsultationApi.Infrastructure.RabbitMQ.Config;
using ConsultationApi.Infrastructure.RabbitMQ.Publishers;
using ConsultationApi.Domain.Interfaces.Redis;
using ConsultationApi.Domain.Interfaces.Messaging;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Repositories.Users;
using ConsultationApi.Infrastructure.Repositories.Doctors;
using ConsultationApi.Infrastructure.Repositories.Appointments;
using ConsultationApi.Infrastructure.Repositories.Chat;
using ConsultationApi.Infrastructure.Repositories.Notifications;
using ConsultationApi.Infrastructure.Repositories.Reviews;
using ConsultationApi.Infrastructure.SignalR;

namespace ConsultationApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<AppDbContext>(
            options =>
                options.UseNpgsql(
                    configuration
                        .GetConnectionString(
                            "DefaultConnection")));

        // HttpContext
        services.AddHttpContextAccessor();

        // Authentication Helpers
        services.AddScoped<
            ConsultationApi.Domain.Interfaces.Authentication.IPasswordHasher,
            PasswordHasher>();

        services.AddScoped<
            ConsultationApi.Domain.Interfaces.Authentication.IJwtTokenGenerator,
            JwtTokenGenerator>();

        // Services
        services.AddScoped<
            ConsultationApi.Domain.Interfaces.Services.ICurrentUserService,
            CurrentUserService>();

        services.AddSingleton<
            DateTimeProvider>();

        services.AddSingleton<
            RedisConnectionFactory>();

        // Expose StackExchange.Redis connection for components that depend on it
        services.AddSingleton<
            StackExchange.Redis.IConnectionMultiplexer>(
            sp => sp.GetRequiredService<RedisConnectionFactory>().Connection);

        services.AddScoped<
            ConsultationApi.Domain.Interfaces.Redis.ICacheService,
            CacheService>();

        services.Configure<RabbitMqSettings>(
            configuration.GetSection(
                "RabbitMq"));

        services.AddSingleton<
            IRabbitMqPublisher,
            RabbitMqPublisher>();

        services.AddScoped<
            IUserRepository,
            UserRepository>();

        services.AddScoped<
            IDoctorRepository,
            DoctorRepository>();

        services.AddScoped<
            ISlotRepository,
            SlotRepository>();

        services.AddScoped<
            IAppointmentRepository,
            AppointmentRepository>();

        services.AddScoped<
            IChatRepository,
            ChatRepository>();

        services.AddScoped<
            INotificationRepository,
            NotificationRepository>();

        services.AddScoped<
            IReviewRepository,
            ReviewRepository>();

        services.AddSingleton<
            ConnectionTracker>();

        services.AddSingleton<
            IPresenceService,
            PresenceService>();

        return services;
    }
}