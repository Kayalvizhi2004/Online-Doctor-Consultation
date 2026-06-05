using AutoMapper;
using ConsultationApi.Application.DTOs.Appointments;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.Interfaces.Appointments;
using ConsultationApi.Domain.Entities.Appointments;
using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Enums;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Domain.Interfaces.Messaging;
using ConsultationApi.Domain.Interfaces.Redis;
using ConsultationApi.Domain.Events.Appointments;

namespace ConsultationApi.Application.Services.Appointments;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointments;
    private readonly ISlotRepository _slots;
    private readonly IUserRepository _users;
    private readonly ICacheService _cache;
    private readonly IRabbitMqPublisher _publisher;
    private readonly IMapper _mapper;

    public AppointmentService(
        IAppointmentRepository appointments,
        ISlotRepository slots,
        IUserRepository users,
        ICacheService cache,
        IRabbitMqPublisher publisher,
        IMapper mapper)
    {
        _appointments = appointments;
        _slots = slots;
        _users = users;
        _cache = cache;
        _publisher = publisher;
        _mapper = mapper;
    }

    public async Task<ApiResponse<string>> BookAppointmentAsync(
        Guid patientId,
        BookAppointmentDto dto)
    {
        var slot =
            await _slots.GetByIdAsync(dto.SlotId);

        if (slot == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Slot not found"
            };
        }

        if (slot.IsBooked)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Slot already booked"
            };
        }

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = slot.DoctorId,
            SlotId = slot.Id,
            Notes = dto.Notes,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        slot.IsBooked = true;

        await _appointments.AddAsync(appointment);
        await _slots.UpdateAsync(slot);
        await _appointments.SaveChangesAsync();

        await _cache.RemoveByPatternAsync(
            $"doctors:{slot.DoctorId}:slots");

        var patient =
            await _users.GetByIdAsync(patientId);

        var evt = new
        {
            eventId = Guid.NewGuid(),
            appointmentId = appointment.Id,
            patientId = patientId,
            patientName = patient!.FullName,
            doctorId = slot.DoctorId,
            slotDate = slot.Date,
            slotTime = slot.StartTime.ToString(),
            occurredAt = DateTime.UtcNow
        };

        _publisher.Publish(evt, "appointment.booked");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Appointment booked"
        };
    }

    public async Task<ApiResponse<PagedResponse<AppointmentDto>>>
        GetAppointmentsAsync(
            Guid userId,
            string role,
            AppointmentFilterDto filter)
    {
        var appointments =
            await _appointments.GetAppointmentsAsync(
                userId,
                role,
                filter.Status,
                filter.Page,
                filter.PageSize);

        return new ApiResponse<
            PagedResponse<AppointmentDto>>
        {
            Success = true,
            Data = new PagedResponse<AppointmentDto>
            {
                Items = _mapper.Map<
                    IEnumerable<AppointmentDto>>(
                        appointments)
            }
        };
    }

    public async Task<ApiResponse<AppointmentDto>>
        GetAppointmentAsync(Guid appointmentId, Guid userId)
    {
        var appointment =
            await _appointments.GetByIdAsync(appointmentId);

        if (appointment == null)
        {
            return new ApiResponse<AppointmentDto>
            {
                Success = false,
                Message = "Appointment not found"
            };
        }

        return new ApiResponse<AppointmentDto>
        {
            Success = true,
            Data = _mapper.Map<
                AppointmentDto>(appointment)
        };
    }

    public async Task<ApiResponse<string>>
        ConfirmAppointmentAsync(
            Guid doctorId,
            Guid appointmentId)
    {
        var appointment =
            await _appointments.GetByIdAsync(
                appointmentId);

        if (appointment == null)
        {
            return new ApiResponse<string>
            {
                Success = false
            };
        }

        appointment.Status =
            AppointmentStatus.Confirmed;

        await _appointments.UpdateAsync(
            appointment);
        await _appointments.SaveChangesAsync();

        var patient =
            await _users.GetByIdAsync(appointment.PatientId);

        var evt = new
        {
            appointmentId = appointment.Id,
            patientId = appointment.PatientId,
            patientName = patient?.FullName ?? "Patient",
            doctorId = appointment.DoctorId
        };

        _publisher.Publish(evt, "appointment.confirmed");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Confirmed"
        };
    }

    public async Task<ApiResponse<string>>
        CancelAppointmentAsync(
            Guid userId,
            Guid appointmentId)
    {
        var appointment =
            await _appointments.GetByIdAsync(
                appointmentId);

        if (appointment == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Appointment not found"
            };
        }

        // Determine if user is patient or doctor
        bool isPatient = appointment.PatientId == userId;
        bool isDoctor = appointment.DoctorId == userId;

        if (!isPatient && !isDoctor)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Unauthorized to cancel this appointment"
            };
        }

        // If patient, check 1-hour rule
        if (isPatient && appointment.Slot != null)
        {
            var appointmentDateTime = appointment.Slot.Date.ToDateTime(appointment.Slot.StartTime);
            var timeUntilAppointment = appointmentDateTime - DateTime.UtcNow;

            if (timeUntilAppointment.TotalHours <= 1)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Cannot cancel appointment within 1 hour of session start"
                };
            }
        }

        appointment.Status =
            AppointmentStatus.Cancelled;

        await _appointments.UpdateAsync(
            appointment);
        await _appointments.SaveChangesAsync();

        var cancellationReason = isPatient ? "Patient requested cancellation" : "Doctor cancelled appointment";
        var cancelledBy = isPatient ? "Patient" : "Doctor";

        var evt = new
        {
            appointmentId = appointment.Id,
            patientId = appointment.PatientId,
            doctorId = appointment.DoctorId,
            cancellationReason = cancellationReason,
            cancelledBy = cancelledBy
        };

        _publisher.Publish(evt, "appointment.cancelled");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Cancelled"
        };
    }

    public async Task<ApiResponse<Guid>>
        StartSessionAsync(
            Guid doctorId,
            Guid appointmentId)
    {
        var appointment =
            await _appointments.GetByIdAsync(
                appointmentId);

        if (appointment == null)
        {
            return new ApiResponse<Guid>
            {
                Success = false
            };
        }

        appointment.Status =
            AppointmentStatus.InProgress;

        var session =
            new ConsultationSession
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointmentId,
                StartedAt = DateTime.UtcNow
            };

        await _appointments
            .CreateSessionAsync(session);
        await _appointments.UpdateAsync(appointment);
        await _appointments.SaveChangesAsync();

        await _cache.SetAsync(
            $"session:active:{session.Id}",
            true,
            TimeSpan.FromMinutes(30));

        return new ApiResponse<Guid>
        {
            Success = true,
            Data = session.Id
        };
    }

    public async Task<ApiResponse<string>>
        EndSessionAsync(
            Guid doctorId,
            Guid appointmentId)
    {
        var session =
            await _appointments
                .GetSessionByAppointmentAsync(
                    appointmentId);

        if (session == null)
        {
            return new ApiResponse<string>
            {
                Success = false
            };
        }

        session.EndedAt =
            DateTime.UtcNow;

        session.Summary = string.Empty;

        var appointment =
            await _appointments.GetByIdAsync(
                session.AppointmentId);

        if (appointment != null)
        {
            appointment.Status =
                AppointmentStatus.Completed;
            await _appointments.UpdateAsync(
                appointment);
        }

        await _appointments
            .UpdateSessionAsync(
                session);
        await _appointments.SaveChangesAsync();

        await _cache.RemoveAsync(
            $"session:active:{session.Id}");

        var evt = new
        {
            appointmentId = session.AppointmentId,
            patientId = appointment?.PatientId
        };

        _publisher.Publish(evt, "consultation.completed");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Session ended"
        };
    }
}