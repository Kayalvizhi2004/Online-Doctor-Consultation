using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Enums;

namespace ConsultationApi.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // ----------------------------
        // PATIENT USERS
        // ----------------------------

        var patient1Id = Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

        var patient2Id = Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

        // ----------------------------
        // DOCTOR USERS
        // ----------------------------

        var doctorUser1Id = Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

        var doctorUser2Id = Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

        var doctorUser3Id = Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

        // ----------------------------
        // DOCTOR PROFILES
        // ----------------------------

        var doctorProfile1Id = Guid.Parse(
            "66666666-6666-6666-6666-666666666666");

        var doctorProfile2Id = Guid.Parse(
            "77777777-7777-7777-7777-777777777777");

        var doctorProfile3Id = Guid.Parse(
            "88888888-8888-8888-8888-888888888888");

        // ----------------------------
        // USERS
        // ----------------------------

        modelBuilder.Entity<User>().HasData(

            new User
            {
                Id = patient1Id,
                FullName = "John Patient",
                Email = "patient1@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = UserRole.Patient,
                Phone = "9000000001",
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Id = patient2Id,
                FullName = "Mary Patient",
                Email = "patient2@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = UserRole.Patient,
                Phone = "9000000002",
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Id = doctorUser1Id,
                FullName = "Dr Arun Kumar",
                Email = "doctor1@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = UserRole.Doctor,
                Phone = "9000000003",
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Id = doctorUser2Id,
                FullName = "Dr Priya Sharma",
                Email = "doctor2@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = UserRole.Doctor,
                Phone = "9000000004",
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Id = doctorUser3Id,
                FullName = "Dr Rajesh Singh",
                Email = "doctor3@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = UserRole.Doctor,
                Phone = "9000000005",
                CreatedAt = DateTime.UtcNow
            }
        );

        // ----------------------------
        // DOCTOR PROFILES
        // ----------------------------

        modelBuilder.Entity<DoctorProfile>().HasData(

            new DoctorProfile
            {
                Id = doctorProfile1Id,
                UserId = doctorUser1Id,
                Specialization = "Cardiology",
                Bio = "Heart specialist",
                ConsultationFee = 500,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            },

            new DoctorProfile
            {
                Id = doctorProfile2Id,
                UserId = doctorUser2Id,
                Specialization = "Dermatology",
                Bio = "Skin specialist",
                ConsultationFee = 400,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            },

            new DoctorProfile
            {
                Id = doctorProfile3Id,
                UserId = doctorUser3Id,
                Specialization = "Cardiology",
                Bio = "Senior cardiologist",
                ConsultationFee = 700,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // ----------------------------
        // AVAILABILITY SLOTS
        // ----------------------------

        modelBuilder.Entity<AvailabilitySlot>().HasData(

            new AvailabilitySlot
            {
                Id = Guid.Parse(
                    "90000000-0000-0000-0000-000000000001"),
                DoctorId = doctorProfile1Id,
                Date = new DateOnly(2026, 6, 10),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(10, 30),
                IsBooked = false
            },

            new AvailabilitySlot
            {
                Id = Guid.Parse(
                    "90000000-0000-0000-0000-000000000002"),
                DoctorId = doctorProfile1Id,
                Date = new DateOnly(2026, 6, 10),
                StartTime = new TimeOnly(11, 0),
                EndTime = new TimeOnly(11, 30),
                IsBooked = false
            },

            new AvailabilitySlot
            {
                Id = Guid.Parse(
                    "90000000-0000-0000-0000-000000000003"),
                DoctorId = doctorProfile2Id,
                Date = new DateOnly(2026, 6, 11),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(9, 30),
                IsBooked = false
            },

            new AvailabilitySlot
            {
                Id = Guid.Parse(
                    "90000000-0000-0000-0000-000000000004"),
                DoctorId = doctorProfile3Id,
                Date = new DateOnly(2026, 6, 11),
                StartTime = new TimeOnly(2, 0),
                EndTime = new TimeOnly(2, 30),
                IsBooked = false
            },

            new AvailabilitySlot
            {
                Id = Guid.Parse(
                    "90000000-0000-0000-0000-000000000005"),
                DoctorId = doctorProfile3Id,
                Date = new DateOnly(2026, 6, 12),
                StartTime = new TimeOnly(4, 0),
                EndTime = new TimeOnly(4, 30),
                IsBooked = false
            }
        );
    }
}