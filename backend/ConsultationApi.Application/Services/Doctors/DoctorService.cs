using AutoMapper;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Doctors;
using ConsultationApi.Application.Interfaces.Doctors;
using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Domain.Interfaces.Redis;
// alias CacheKeys from Application.Common to avoid importing the entire namespace
using CacheKeys = ConsultationApi.Application.Common.CacheKeys;

namespace ConsultationApi.Application.Services.Doctors;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly ISlotRepository _slotRepo;
    private readonly ICacheService _cache;
    private readonly IMapper _mapper;

    public DoctorService(
        IDoctorRepository doctorRepo,
        ISlotRepository slotRepo,
        ICacheService cache,
        IMapper mapper)
    {
        _doctorRepo = doctorRepo;
        _slotRepo = slotRepo;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<DoctorDto>>>
        GetDoctorsAsync(
        DoctorFilterDto filter)
    {
        var key =
            CacheKeys.DoctorList(
                filter.Specialization ??
                "all");

        var cached =
            await _cache.GetAsync<
                PagedResponse<DoctorDto>>(key);

        if (cached != null)
        {
            return new ApiResponse<
                PagedResponse<DoctorDto>>
            {
                Success = true,
                Message = "Cache hit",
                Data = cached
            };
        }

        var doctors =
            await _doctorRepo
                .GetDoctorsAsync(
                    filter.Specialization,
                    filter.AvailableDate);

        var dto =
            new PagedResponse<DoctorDto>
            {
                Items = _mapper.Map<
                    IEnumerable<DoctorDto>>(
                        doctors)
            };

        await _cache.SetAsync(
            key,
            dto,
            TimeSpan.FromMinutes(5));

        return new ApiResponse<
            PagedResponse<DoctorDto>>
        {
            Success = true,
            Data = dto
        };
    }

    public async Task<ApiResponse<DoctorDto>>
        GetDoctorByIdAsync(
            Guid doctorId)
    {
        var key =
            CacheKeys.DoctorProfile(
                doctorId);

        var cached =
            await _cache.GetAsync<
                DoctorDto>(key);

        if (cached != null)
        {
            return new ApiResponse<DoctorDto>
            {
                Success = true,
                Message = "Cache hit",
                Data = cached
            };
        }

        var doctor =
            await _doctorRepo
                .GetDoctorByIdAsync(
                    doctorId);

        if (doctor == null)
        {
            return ApiResponse<DoctorDto>.Failure("Doctor not found", 404);
        }

        var dto =
            _mapper.Map<DoctorDto>(
                doctor);

        await _cache.SetAsync(
            key,
            dto,
            TimeSpan.FromMinutes(10));

        return new ApiResponse<DoctorDto>
        {
            Success = true,
            Data = dto
        };
    }

    public async Task<ApiResponse<string>>
        UpdateProfileAsync(
            Guid doctorId,
            DoctorProfileUpdateDto dto)
    {
        var doctor =
            await _doctorRepo
                .GetDoctorByUserIdAsync(
                    doctorId);

        if (doctor == null)
        {
            return ApiResponse<string>.Failure("Doctor not found", 404);
        }

        doctor.Bio = dto.Bio;
        doctor.ConsultationFee =
            dto.ConsultationFee;
        doctor.IsAvailable =
            dto.IsAvailable;

        await _doctorRepo.UpdateAsync(
            doctor);

        await _doctorRepo.SaveChangesAsync();

        await _cache.RemoveByPatternAsync(
            "doctors:");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Profile updated"
        };
    }

    public async Task<ApiResponse<string>>
        AddSlotAsync(
            Guid doctorId,
            CreateSlotDto dto)
    {
        var doctor = await _doctorRepo.GetDoctorByUserIdAsync(doctorId);

        if (doctor == null)
        {
            return ApiResponse<string>.Failure("Doctor not found", 404);
        }

        var slot = new AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            DoctorId = doctor.Id,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsBooked = false
        };


        await _slotRepo.AddSlotAsync(slot);
        await _slotRepo.SaveChangesAsync();

        await _cache.RemoveByPatternAsync("doctors:");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Slot added"
        };
    }

    public async Task<ApiResponse<string>>
        RemoveSlotAsync(
            Guid doctorId,
            Guid slotId)
    {
        var slot =
            await _slotRepo
                .GetByIdAsync(
                    slotId);
        var doctor = await _doctorRepo.GetDoctorByUserIdAsync(doctorId);

        if (doctor == null)
        {
            return ApiResponse<string>.Failure("Doctor not found", 404);
        }

        if (slot == null ||
            slot.DoctorId != doctor.Id)
        {
            return ApiResponse<string>.Failure("Slot not found", 404);
        }

        if (slot.IsBooked)
        {
            return ApiResponse<string>.Failure("Booked slot cannot be removed", 400);
        }


        await _slotRepo.DeleteAsync(slot);
        await _slotRepo.SaveChangesAsync();

        await _cache.RemoveByPatternAsync("doctors:");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Slot removed"
        };
    }

    public async Task<ApiResponse<string>>
        ToggleAvailabilityAsync(
            Guid doctorId,
            bool isAvailable)
    {
        var doctor =
            await _doctorRepo
                .GetDoctorByUserIdAsync(
                    doctorId);

        if (doctor == null)
        {
            return ApiResponse<string>.Failure("Doctor not found", 404);
        }

        doctor.IsAvailable =
            isAvailable;

        await _doctorRepo.UpdateAsync(doctor);
        await _doctorRepo.SaveChangesAsync();

        await _cache.RemoveByPatternAsync("doctors:");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Availability updated"
        };
    }
}