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

        // Clear ALL doctor caches (list + profile + slots) so the booked slot
        // is reflected everywhere immediately. The detail endpoint caches under
        // "doctors:{id}:profile", which a narrower "...:slots" pattern would miss.
        await _cache.RemoveByPatternAsync(
            "doctors:");

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

        var total = await _appointments.GetAppointmentsCountAsync(userId, role, filter.Status);

        return new ApiResponse<
            PagedResponse<AppointmentDto>>
        {
            Success = true,
            Data = new PagedResponse<AppointmentDto>
            {
                Items = _mapper.Map<
                    IEnumerable<AppointmentDto>>(
                        appointments),
                TotalCount = total
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
            return ApiResponse<AppointmentDto>.Failure("Appointment not found", 404);
        }

        // IDOR guard: only this appointment's own patient or doctor may view it.
        // PatientId is a user id; DoctorId is the doctor PROFILE id, so the
        // doctor must be matched through the profile's UserId.
        bool isPatient = appointment.PatientId == userId;
        bool isDoctor = appointment.Doctor != null && appointment.Doctor.UserId == userId;

        if (!isPatient && !isDoctor)
        {
            return ApiResponse<AppointmentDto>.Failure(
                "You do not have access to this appointment", 403);
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
                Success = false,
                Message = "Appointment not found"
            };
        }

        if (appointment.Doctor == null || appointment.Doctor.UserId != doctorId)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "You are not allowed to manage this appointment"
            };
        }

        if (appointment.Status != AppointmentStatus.Pending)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Only pending appointments can be confirmed"
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

        // Determine if user is patient or doctor.
        // Patient id IS a user id, but DoctorId is the doctor PROFILE id, so the
        // doctor check must go through the profile's UserId.
        bool isPatient = appointment.PatientId == userId;
        bool isDoctor = appointment.Doctor != null && appointment.Doctor.UserId == userId;

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

        // Free the slot so it can be booked again.
        if (appointment.Slot != null)
        {
            appointment.Slot.IsBooked = false;
            await _slots.UpdateAsync(appointment.Slot);
        }

        await _appointments.SaveChangesAsync();

        // Doctor list/detail caches embed slot booking state -> refresh them.
        await _cache.RemoveByPatternAsync("doctors:");

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
                Success = false,
                Message = "Appointment not found"
            };
        }

        if (appointment.Doctor == null || appointment.Doctor.UserId != doctorId)
        {
            return new ApiResponse<Guid>
            {
                Success = false,
                Message = "You are not allowed to manage this appointment"
            };
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return new ApiResponse<Guid>
            {
                Success = false,
                Message = "Only confirmed appointments can be started"
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

        // Notify the patient that the consultation has started (via the worker).
        _publisher.Publish(new
        {
            appointmentId = appointment.Id,
            patientId = appointment.PatientId,
            doctorId = appointment.DoctorId,
            sessionId = session.Id,
            occurredAt = DateTime.UtcNow
        }, "consultation.started");

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

        if (appointment == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Appointment not found"
            };
        }

        if (appointment.Doctor == null || appointment.Doctor.UserId != doctorId)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "You are not allowed to manage this appointment"
            };
        }

        if (appointment.Status != AppointmentStatus.InProgress)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Only an in-progress consultation can be ended"
            };
        }

        appointment.Status =
            AppointmentStatus.Completed;
        await _appointments.UpdateAsync(
            appointment);

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