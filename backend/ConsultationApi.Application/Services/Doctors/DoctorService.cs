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
        // The cache key must include EVERY filter (search + specialization +
        // date); keying on specialization alone would serve stale results for
        // different searches. Each free-text segment is URL-encoded so a value
        // containing the '|' separator can't collide with another combination.
        var key =
            CacheKeys.DoctorList(
                $"{Uri.EscapeDataString(filter.Specialization ?? "all")}" +
                $"|{Uri.EscapeDataString(filter.Search?.Trim().ToLowerInvariant() ?? "")}" +
                $"|{filter.AvailableDate?.ToString("yyyy-MM-dd") ?? ""}");

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
                    filter.AvailableDate,
                    filter.Search);

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

    // Distinct specializations present in the data, for the filter dropdown.
    // Not cached: it's a trivial DISTINCT query and the whole point is that a
    // newly added specialization shows up immediately.
    public async Task<ApiResponse<IEnumerable<string>>>
        GetSpecializationsAsync()
    {
        var list = await _doctorRepo.GetSpecializationsAsync();

        return ApiResponse<IEnumerable<string>>
            .SuccessResponse(list);
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

    // Returns the signed-in doctor's own slots. The caller passes the JWT
    // user id; we resolve the owning doctor PROFILE (profiles.Id != users.Id)
    // before reading slots, and skip the cache so a freshly added/edited slot
    // shows up immediately on the management page.
    public async Task<ApiResponse<IEnumerable<AvailabilitySlotDto>>>
        GetMyAvailabilityAsync(
            Guid userId)
    {
        var doctor =
            await _doctorRepo
                .GetDoctorByUserIdAsync(
                    userId);

        if (doctor == null)
        {
            return ApiResponse<IEnumerable<AvailabilitySlotDto>>
                .Failure("Doctor profile not found", 404);
        }

        var slots =
            doctor.AvailabilitySlots
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime);

        var dto =
            _mapper.Map<IEnumerable<AvailabilitySlotDto>>(
                slots);

        return ApiResponse<IEnumerable<AvailabilitySlotDto>>
            .SuccessResponse(dto);
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
        UpdateSlotAsync(
            Guid doctorId,
            Guid slotId,
            CreateSlotDto dto)
    {
        var doctor = await _doctorRepo.GetDoctorByUserIdAsync(doctorId);

        if (doctor == null)
        {
            return ApiResponse<string>.Failure("Doctor not found", 404);
        }

        var slot = await _slotRepo.GetByIdAsync(slotId);

        if (slot == null ||
            slot.DoctorId != doctor.Id)
        {
            return ApiResponse<string>.Failure("Slot not found", 404);
        }

        if (slot.IsBooked)
        {
            return ApiResponse<string>.Failure("A booked slot cannot be edited", 400);
        }

        slot.Date = dto.Date;
        slot.StartTime = dto.StartTime;
        slot.EndTime = dto.EndTime;

        await _slotRepo.UpdateAsync(slot);
        await _slotRepo.SaveChangesAsync();

        await _cache.RemoveByPatternAsync("doctors:");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Slot updated"
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

