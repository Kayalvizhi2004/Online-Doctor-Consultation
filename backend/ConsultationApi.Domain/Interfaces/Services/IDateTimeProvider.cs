namespace ConsultationApi.Domain.Interfaces.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}