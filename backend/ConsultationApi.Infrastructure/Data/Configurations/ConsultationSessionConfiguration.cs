using ConsultationApi.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultationApi.Infrastructure.Data.Configurations;

public class ConsultationSessionConfiguration
    : IEntityTypeConfiguration<ConsultationSession>
{
    public void Configure(
        EntityTypeBuilder<ConsultationSession> builder)
    {
        builder.ToTable(
            "consultation_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.AppointmentId)
            .HasColumnName("appointment_id");

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at");

        builder.Property(x => x.EndedAt)
            .HasColumnName("ended_at");

        builder.Property(x => x.Summary)
            .HasColumnName("summary");

        builder.HasOne(x => x.Appointment)
            .WithOne(x => x.ConsultationSession)
            .HasForeignKey<ConsultationSession>(
                x => x.AppointmentId);
    }
}