using ConsultationApi.Domain.Entities.Appointments;
using ConsultationApi.Domain.Entities.Chat;
using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Entities.Notifications;
using ConsultationApi.Domain.Entities.Reviews;
using ConsultationApi.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {
    }

    // USERS
    public DbSet<User> Users => Set<User>();

    // DOCTORS
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();

    public DbSet<AvailabilitySlot> AvailabilitySlots
        => Set<AvailabilitySlot>();

    // APPOINTMENTS
    public DbSet<Appointment> Appointments
        => Set<Appointment>();

    public DbSet<ConsultationSession> ConsultationSessions
        => Set<ConsultationSession>();

    // CHAT
    public DbSet<ChatMessage> ChatMessages
        => Set<ChatMessage>();

    // NOTIFICATIONS
    public DbSet<Notification> Notifications
        => Set<Notification>();

    // REVIEWS
    public DbSet<Review> Reviews
        => Set<Review>();

    // AUTH - Refresh tokens
    public DbSet<ConsultationApi.Domain.Entities.Users.RefreshToken> RefreshTokens
        => Set<ConsultationApi.Domain.Entities.Users.RefreshToken>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Load all Fluent Configurations automatically
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );

        // Apply seed data defined in DbSeeder
        try
        {
            ConsultationApi.Infrastructure.Data.Seed.DbSeeder.Seed(modelBuilder);
        }
        catch
        {
            // swallow to avoid breaking design-time tools when seed types are incompatible
        }
    }
}