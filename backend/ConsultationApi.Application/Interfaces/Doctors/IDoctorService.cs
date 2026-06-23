using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Doctors;

namespace ConsultationApi.Application.Interfaces.Doctors;

public interface IDoctorService
{
    Task<ApiResponse<PagedResponse<DoctorDto>>> GetDoctorsAsync(
        DoctorFilterDto filter);

    Task<ApiResponse<IEnumerable<string>>> GetSpecializationsAsync();

    Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(
        Guid doctorId);

    Task<ApiResponse<string>> UpdateProfileAsync(
        Guid doctorId,
        DoctorProfileUpdateDto dto);

    Task<ApiResponse<IEnumerable<AvailabilitySlotDto>>> GetMyAvailabilityAsync(
        Guid userId);

    Task<ApiResponse<string>> AddSlotAsync(
        Guid doctorId,
        CreateSlotDto dto);

    Task<ApiResponse<string>> UpdateSlotAsync(
        Guid doctorId,
        Guid slotId,
        CreateSlotDto dto);

    Task<ApiResponse<string>> RemoveSlotAsync(
        Guid doctorId,
        Guid slotId);

    Task<ApiResponse<string>> ToggleAvailabilityAsync(
        Guid doctorId,
        bool isAvailable);
}