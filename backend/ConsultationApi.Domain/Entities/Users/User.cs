using ConsultationApi.Domain.Common;
using ConsultationApi.Domain.Enums;
using ConsultationApi.Domain.Entities.Doctors;

namespace ConsultationApi.Domain.Entities.Users;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string Phone { get; set; } = string.Empty;

    public DoctorProfile? DoctorProfile { get; set; }
}