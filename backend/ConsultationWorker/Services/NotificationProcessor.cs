using System;
using System.Text.Json;
using ConsultationApi.Domain.Entities.Notifications;
using ConsultationApi.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsultationWorker.Services;

public class NotificationProcessor
{
    private readonly IServiceProvider _provider;
    private readonly EmailSimulator _emails;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        IServiceProvider provider,
        EmailSimulator emails,
        ILogger<NotificationProcessor> logger)
    {
        _provider = provider;
        _emails = emails;
        _logger = logger;
    }

    public async Task ProcessAsync(string routingKey, byte[] body, CancellationToken ct)
    {
        var payload = System.Text.Encoding.UTF8.GetString(body);

        _logger.LogInformation("Processing event {RoutingKey}: {Payload}", routingKey, payload);

        try
        {
            using var scope = _provider.CreateScope();
            var notifications = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            Guid? patientId = null;
            Guid? doctorId = null;

            if (root.TryGetProperty("patientId", out var p) && p.ValueKind == JsonValueKind.String)
                patientId = Guid.Parse(p.GetString()!);

            if (root.TryGetProperty("doctorId", out var d) && d.ValueKind == JsonValueKind.String)
                doctorId = Guid.Parse(d.GetString()!);

            var title = routingKey;
            var message = payload;

            if (patientId.HasValue)
            {
                var n = new Notification
                {
                    UserId = patientId.Value,
                    Title = title,
                    Message = message,
                    Type = ConsultationApi.Domain.Enums.NotificationType.Appointment,
                    IsRead = false
                };

                await notifications.AddAsync(n);
            }

            if (doctorId.HasValue)
            {
                var n2 = new Notification
                {
                    UserId = doctorId.Value,
                    Title = title,
                    Message = message,
                    Type = ConsultationApi.Domain.Enums.NotificationType.Appointment,
                    IsRead = false
                };

                await notifications.AddAsync(n2);
            }

            await notifications.SaveChangesAsync();

            await _emails.SendEmailAsync("admin@local", title, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process event {RoutingKey}", routingKey);
            throw;
        }
    }
}
