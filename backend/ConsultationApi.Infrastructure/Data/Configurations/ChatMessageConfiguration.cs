using ConsultationApi.Domain.Entities.Chat;
using ConsultationApi.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultationApi.Infrastructure.Data.Configurations;

public class ChatMessageConfiguration
    : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(
        EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id");

        builder.Property(x => x.SenderId)
            .HasColumnName("sender_id");

        builder.Property(x => x.Message)
            .HasColumnName("message")
            .IsRequired();

        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at")
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.IsRead)
            .HasColumnName("is_read");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}