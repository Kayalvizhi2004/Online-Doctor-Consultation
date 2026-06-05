namespace ConsultationApi.Domain.Interfaces.Messaging;

public interface IRabbitMqPublisher
{
    void Publish<T>(
        T message,
        string routingKey);
}