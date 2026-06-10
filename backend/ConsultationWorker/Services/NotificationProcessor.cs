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

        _logger.LogInformation("[NotificationProcessor] Processing event {RoutingKey}: {Payload}", routingKey, payload);

        try
        {
            // Create a new scope for each message to ensure fresh DbContext
            using var scope = _provider.CreateScope();
            var notifications = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var doctors = scope.ServiceProvider.GetRequiredService<IDoctorRepository>();

            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            Guid? patientId = null;
            Guid? doctorId = null;
            string? patientName = null;
            string? cancellationReason = null;
            string? cancelledBy = null;

            if (root.TryGetProperty("patientId", out var p) && p.ValueKind == JsonValueKind.String)
                patientId = Guid.Parse(p.GetString()!);

            if (root.TryGetProperty("doctorId", out var d) && d.ValueKind == JsonValueKind.String)
                doctorId = Guid.Parse(d.GetString()!);

            if (root.TryGetProperty("patientName", out var pn) && pn.ValueKind == JsonValueKind.String)
                patientName = pn.GetString();

            if (root.TryGetProperty("cancellationReason", out var cr) && cr.ValueKind == JsonValueKind.String)
                cancellationReason = cr.GetString();

            if (root.TryGetProperty("cancelledBy", out var cb) && cb.ValueKind == JsonValueKind.String)
                cancelledBy = cb.GetString();

            string title;
            string message;
            var notificationType = ConsultationApi.Domain.Enums.NotificationType.Appointment;

            switch (routingKey)
            {
                case "appointment.booked":
                    // Doctor gets: "New appointment request from [Patient]"
                    title = "New Appointment Request";
                    message = $"New appointment request from {patientName ?? "a patient"}";
                    
                    if (doctorId.HasValue)
                    {
                        // doctorId in the event is the doctor PROFILE id; notifications
                        // target the doctor's USER id.
                        var doctorUserId = await ResolveDoctorUserId(doctors, doctorId.Value);
                        if (doctorUserId.HasValue)
                        {
                            await notifications.AddAsync(new Notification
                            {
                                Id = Guid.NewGuid(),
                                UserId = doctorUserId.Value,
                                Title = title,
                                Message = message,
                                Type = notificationType,
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow
                            });
                            _logger.LogInformation("[NotificationProcessor] Added notification for doctor user {UserId}", doctorUserId);
                        }
                    }
                    break;

                case "consultation.started":
                    // Patient gets: consultation has started, join now
                    title = "Consultation Started";
                    message = "Your consultation has started. Open it to join the chat.";

                    if (patientId.HasValue)
                    {
                        await notifications.AddAsync(new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = patientId.Value,
                            Title = title,
                            Message = message,
                            Type = notificationType,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        });
                        _logger.LogInformation("[NotificationProcessor] Added session-started notification for patient {PatientId}", patientId);
                    }
                    break;

                case "appointment.confirmed":
                    // Patient gets: "Your appointment is confirmed"
                    title = "Appointment Confirmed";
                    message = "Your appointment has been confirmed by the doctor";
                    
                    if (patientId.HasValue)
                    {
                        var patientNotif = new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = patientId.Value,
                            Title = title,
                            Message = message,
                            Type = notificationType,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        await notifications.AddAsync(patientNotif);
                        _logger.LogInformation("[NotificationProcessor] Added notification for patient {PatientId}", patientId);
                    }
                    break;

                case "appointment.cancelled":
                    // Both get notification with cancellation details
                    title = "Appointment Cancelled";
                    var canceller = cancelledBy ?? "Unknown";
                    var reason = cancellationReason ?? "No reason provided";
                    message = $"Appointment has been cancelled by {canceller}. Reason: {reason}";
                    
                    if (patientId.HasValue)
                    {
                        var patientNotif = new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = patientId.Value,
                            Title = title,
                            Message = message,
                            Type = notificationType,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        await notifications.AddAsync(patientNotif);
                        _logger.LogInformation("[NotificationProcessor] Added cancellation notification for patient {PatientId}", patientId);
                    }

                    if (doctorId.HasValue)
                    {
                        var doctorUserId = await ResolveDoctorUserId(doctors, doctorId.Value);
                        if (doctorUserId.HasValue)
                        {
                            await notifications.AddAsync(new Notification
                            {
                                Id = Guid.NewGuid(),
                                UserId = doctorUserId.Value,
                                Title = title,
                                Message = message,
                                Type = notificationType,
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow
                            });
                            _logger.LogInformation("[NotificationProcessor] Added cancellation notification for doctor user {UserId}", doctorUserId);
                        }
                    }
                    break;

                case "consultation.completed":
                    // Patient gets: Prompt to leave a review
                    title = "Session Completed";
                    message = "Your consultation session has been completed. Please leave a review for the doctor";
                    notificationType = ConsultationApi.Domain.Enums.NotificationType.Review;
                    
                    if (patientId.HasValue)
                    {
                        var patientNotif = new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = patientId.Value,
                            Title = title,
                            Message = message,
                            Type = notificationType,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        await notifications.AddAsync(patientNotif);
                        _logger.LogInformation("[NotificationProcessor] Added review notification for patient {PatientId}", patientId);
                    }
                    break;

                default:
                    _logger.LogWarning("[NotificationProcessor] Unknown routing key: {RoutingKey}", routingKey);
                    return;
            }
        
            // Persist to database
            await notifications.SaveChangesAsync();
            _logger.LogInformation("[NotificationProcessor] Notifications persisted to database for routing key {RoutingKey}", routingKey);

            // Send email notification
            try
            {
                await _emails.SendEmailAsync("admin@local", title, message);
                _logger.LogInformation("[NotificationProcessor] Email sent for {Title}", title);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "[NotificationProcessor] Failed to send email, but continuing");
                // Don't throw - email failure shouldn't cause message reprocessing
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationProcessor] Failed to process event {RoutingKey}", routingKey);
            throw; // Let the consumer handle this with DLQ
        }
    }

    /// <summary>
    /// Events carry the doctor PROFILE id; resolve it to the doctor's USER id
    /// (notifications.user_id references users.id).
    /// </summary>
    private static async Task<Guid?> ResolveDoctorUserId(IDoctorRepository doctors, Guid doctorProfileId)
    {
        var profile = await doctors.GetDoctorByIdAsync(doctorProfileId);
        return profile?.UserId;
    }
}

