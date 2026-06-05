using RabbitMQ.Client;

namespace ConsultationWorker.RabbitMQ;

/// <summary>
/// Configures the Dead Letter Exchange (DLX) for failed messages.
/// Messages that fail processing are sent to the DLQ.
/// </summary>
public static class DeadLetterSetup
{
    /// <summary>
    /// Declares the DLX exchange and DLQ queue (idempotent operations).
    /// </summary>
    /// <param name="channel">RabbitMQ channel</param>
    public static void Configure(IModel channel)
    {
        // Declare DLX (Direct Exchange for dead letters)
        channel.ExchangeDeclare(
            exchange: "appointment.dlx",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Declare DLQ (Dead Letter Queue)
        channel.QueueDeclare(
            queue: "appointment.dlq",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind DLQ to DLX
        channel.QueueBind(
            queue: "appointment.dlq",
            exchange: "appointment.dlx",
            routingKey: "deadletter");
    }
}