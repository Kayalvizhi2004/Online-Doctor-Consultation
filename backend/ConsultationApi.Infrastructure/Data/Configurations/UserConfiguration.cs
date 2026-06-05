using ConsultationApi.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultationApi.Infrastructure.Data.Configurations;

public class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .HasDatabaseName("ix_users_email")
            .IsUnique();
    }
}