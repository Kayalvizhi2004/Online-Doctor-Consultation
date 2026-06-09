using ConsultationApi.Domain.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultationApi.Infrastructure.Data.Configurations;

public class AvailabilitySlotConfiguration
    : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(
        EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.ToTable("availability_slots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DoctorId)
            .HasColumnName("doctor_id");

        builder.Property(x => x.Date)
            .HasColumnName("date");

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time");

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time");

        builder.Property(x => x.IsBooked)
            .HasColumnName("is_booked");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd(); // Tells EF not to send its own default (MinValue)

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(x => x.Doctor)
            .WithMany(x => x.AvailabilitySlots)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}