namespace ConsultationApi.Infrastructure.Redis;

public static class CacheKeys
{
    public static string DoctorList(
        string filterHash)
            => $"doctors:list:{filterHash}";

    public static string DoctorProfile(
        Guid doctorId)
            => $"doctors:{doctorId}:profile";

    public static string DoctorSlots(
        Guid doctorId,
        DateOnly date)
            => $"doctors:{doctorId}:slots:{date}";

    public static string ActiveSession(
        Guid sessionId)
            => $"session:active:{sessionId}";

    public static string UserToken(
        Guid userId)
            => $"user:token:{userId}";

    public static string SignalRConnection(
        Guid userId)
            => $"signalr:user:{userId}";
}