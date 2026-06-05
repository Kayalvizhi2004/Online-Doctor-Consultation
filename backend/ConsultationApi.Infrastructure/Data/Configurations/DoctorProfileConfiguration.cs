using ConsultationApi.Domain.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultationApi.Infrastructure.Data.Configurations;

public class DoctorProfileConfiguration
    : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(
        EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.ToTable("doctor_profiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Specialization)
            .HasColumnName("specialization")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasColumnName("bio");

        builder.Property(x => x.ConsultationFee)
            .HasColumnName("consultation_fee")
            .HasColumnType("numeric(10,2)");

        builder.Property(x => x.IsAvailable)
            .HasColumnName("is_available");

        builder.HasOne(x => x.User)
            .WithOne(x => x.DoctorProfile)
            .HasForeignKey<DoctorProfile>(
                x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}