using ConsultationApi.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultationApi.Infrastructure.Data.Configurations;

public class AppointmentConfiguration
    : IEntityTypeConfiguration<Appointment>
{
    public void Configure(
        EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id");

        builder.Property(x => x.DoctorId)
            .HasColumnName("doctor_id");

        builder.Property(x => x.SlotId)
            .HasColumnName("slot_id");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(x => x.Notes)
            .HasColumnName("notes");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName(
                "ix_appointments_patient_id");

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName(
                "ix_appointments_doctor_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName(
                "ix_appointments_status");
    }
}