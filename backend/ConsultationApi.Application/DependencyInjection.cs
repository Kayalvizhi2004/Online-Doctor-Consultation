using System.Reflection;
using ConsultationApi.Application.Interfaces.Appointments;
using ConsultationApi.Application.Interfaces.Auth;
using ConsultationApi.Application.Interfaces.Chat;
using ConsultationApi.Application.Interfaces.Doctors;
using ConsultationApi.Application.Interfaces.Notifications;
using ConsultationApi.Application.Interfaces.Reviews;
using ConsultationApi.Application.Services.Appointments;
using ConsultationApi.Application.Services.Auth;
using ConsultationApi.Application.Services.Chat;
using ConsultationApi.Application.Services.Doctors;
using ConsultationApi.Application.Services.Notifications;
using ConsultationApi.Application.Services.Reviews;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultationApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection
        AddApplication(
            this IServiceCollection services)
    {
        services.AddAutoMapper(
            Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly());

        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IDoctorService,
            DoctorService>();

        services.AddScoped<
            IAppointmentService,
            AppointmentService>();

        services.AddScoped<
            IChatService,
            ChatService>();

        services.AddScoped<
            INotificationService,
            NotificationService>();

        services.AddScoped<
            IReviewService,
            ReviewService>();

        return services;
    }
}