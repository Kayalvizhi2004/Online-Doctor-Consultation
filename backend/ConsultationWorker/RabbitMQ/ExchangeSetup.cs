using RabbitMQ.Client;

namespace ConsultationWorker.RabbitMQ;

/// <summary>
/// Declares the main RabbitMQ exchange for appointment events.
/// </summary>
public static class ExchangeSetup
{
    /// <summary>
    /// Declares "appointment.exchange" as a direct exchange (idempotent operation).
    /// </summary>
    /// <param name="channel">RabbitMQ channel</param>
    public static void Configure(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: "appointment.exchange",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);
    }
}