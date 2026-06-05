using ConsultationApi.Application.DTOs.Appointments;
using ConsultationApi.Application.DTOs.Common;

namespace ConsultationApi.Application.Interfaces.Appointments;

public interface IAppointmentService
{
    Task<ApiResponse<string>> BookAppointmentAsync(
        Guid patientId,
        BookAppointmentDto dto);

    Task<ApiResponse<PagedResponse<AppointmentDto>>> GetAppointmentsAsync(
        Guid userId,
        string role,
        AppointmentFilterDto filter);

    Task<ApiResponse<AppointmentDto>> GetAppointmentAsync(
        Guid appointmentId,
        Guid userId);

    Task<ApiResponse<string>> ConfirmAppointmentAsync(
        Guid doctorId,
        Guid appointmentId);

    Task<ApiResponse<string>> CancelAppointmentAsync(
        Guid userId,
        Guid appointmentId);

    Task<ApiResponse<Guid>> StartSessionAsync(
        Guid doctorId,
        Guid appointmentId);

    Task<ApiResponse<string>> EndSessionAsync(
        Guid doctorId,
        Guid appointmentId);
}