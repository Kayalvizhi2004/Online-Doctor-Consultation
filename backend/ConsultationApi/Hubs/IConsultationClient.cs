namespace ConsultationApi.Hubs;

public interface IConsultationClient
{
    Task ReceiveMessage(
        object payload);

    Task UserJoined(
        object payload);

    Task SessionEnded(
        object payload);
}