using AutoMapper;
using ConsultationApi.Application.DTOs.Appointments;
using ConsultationApi.Application.DTOs.Auth;
using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.DTOs.Doctors;
using ConsultationApi.Application.DTOs.Notifications;
using ConsultationApi.Application.DTOs.Reviews;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Entities.Appointments;
using ConsultationApi.Domain.Entities.Chat;
using ConsultationApi.Domain.Entities.Notifications;
using ConsultationApi.Domain.Entities.Reviews;

namespace ConsultationApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //------------------------------------------------
        // User → Profile DTO
        //------------------------------------------------

        CreateMap<User, UserProfileDto>();


        //------------------------------------------------
        // DoctorProfile ↔ DoctorDto
        //------------------------------------------------

        CreateMap<DoctorProfile, DoctorDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    src.User != null ? src.User.FullName : string.Empty))
            .ForMember(
                dest => dest.Specialization,
                opt => opt.MapFrom(src =>
                    src.Specialization))
            .ForMember(
                dest => dest.Bio,
                opt => opt.MapFrom(src =>
                    src.Bio))
            .ForMember(
                dest => dest.ConsultationFee,
                opt => opt.MapFrom(src =>
                    src.ConsultationFee))
            .ForMember(
                dest => dest.IsAvailable,
                opt => opt.MapFrom(src =>
                    src.IsAvailable));

        // Map availability slots into the doctor DTO (ensure slots appear)
        CreateMap<DoctorProfile, DoctorDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.ConsultationFee, opt => opt.MapFrom(src => src.ConsultationFee))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
            .ForMember(dest => dest.Slots, opt => opt.MapFrom(src => src.AvailabilitySlots));

        // Map availability slots into the doctor DTO
        CreateMap<DoctorProfile, DoctorDetailDto>()
            .IncludeBase<DoctorProfile, DoctorDto>()
            .ForMember(
                dest => dest.Slots,
                opt => opt.MapFrom(src => src.AvailabilitySlots));


        //------------------------------------------------
        // Slot ↔ DTO
        //------------------------------------------------

        CreateMap<AvailabilitySlot,
            AvailabilitySlotDto>();


        //------------------------------------------------
        // Appointment → DTO
        //------------------------------------------------

        CreateMap<Appointment,
            AppointmentDto>()
            .ForMember(
                dest => dest.DoctorName,
                opt => opt.MapFrom(src =>
                    src.Doctor != null && src.Doctor.User != null ? src.Doctor.User.FullName : string.Empty))
            .ForMember(
                dest => dest.Specialization,
                opt => opt.MapFrom(src =>
                    src.Doctor != null ? src.Doctor.Specialization : string.Empty))
            .ForMember(
                dest => dest.ConsultationFee,
                opt => opt.MapFrom(src =>
                    src.Doctor != null ? src.Doctor.ConsultationFee : 0))
            .ForMember(
                dest => dest.PatientName,
                opt => opt.MapFrom(src =>
                    src.Patient != null ? src.Patient.FullName : string.Empty))
            .ForMember(
                dest => dest.Date,
                opt => opt.MapFrom(src =>
                    src.Slot != null ? (DateOnly?)src.Slot.Date : null))
            .ForMember(
                dest => dest.StartTime,
                opt => opt.MapFrom(src =>
                    src.Slot != null ? (TimeOnly?)src.Slot.StartTime : null))
            .ForMember(
                dest => dest.EndTime,
                opt => opt.MapFrom(src =>
                    src.Slot != null ? (TimeOnly?)src.Slot.EndTime : null))
            .ForMember(
                dest => dest.DoctorId,
                opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src =>
                    src.Status.ToString()));


        //------------------------------------------------
        // ChatMessage → DTO
        //------------------------------------------------

        CreateMap<ChatMessage,
            ChatMessageDto>()
            .ForMember(
                dest => dest.SenderId,
                opt => opt.MapFrom(src => src.SenderId))
            .ForMember(
                dest => dest.SenderName,
                opt => opt.MapFrom(src => src.Sender != null ? src.Sender.FullName : string.Empty))
            .ForMember(
                dest => dest.Message,
                opt => opt.MapFrom(src =>
                    src.Message))
            .ForMember(
                dest => dest.SentAt,
                opt => opt.MapFrom(src =>
                    src.SentAt))
            .ForMember(
                dest => dest.IsRead,
                opt => opt.MapFrom(src =>
                    src.IsRead));


        //------------------------------------------------
        // Notification → DTO
        //------------------------------------------------

        CreateMap<Notification,
            NotificationDto>();


        //------------------------------------------------
        // Review → DTO
        //------------------------------------------------

        CreateMap<Review,
            ReviewDto>()
            .ForMember(
                dest => dest.PatientId,
                opt => opt.MapFrom(src => src.PatientId))
            .ForMember(
                dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
            .ForMember(
                dest => dest.Rating,
                opt => opt.MapFrom(src =>
                    src.Rating))
            .ForMember(
                dest => dest.Comment,
                opt => opt.MapFrom(src =>
                    src.Comment))
            .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src =>
                    src.CreatedAt));
    }
}

