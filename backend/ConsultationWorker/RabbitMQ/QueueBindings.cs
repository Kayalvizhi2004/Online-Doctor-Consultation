using RabbitMQ.Client;

namespace ConsultationWorker.RabbitMQ;

/// <summary>
/// Configures all appointment event queues with proper DLX binding.
/// Each queue is configured to automatically forward failed messages to the DLQ.
/// </summary>
public static class QueueBindings
{
    /// <summary>
    /// Configures all 4 appointment event queues with DLX bindings (idempotent).
    /// </summary>
    /// <param name="channel">RabbitMQ channel</param>
    public static void Configure(IModel channel)
    {
        ConfigureQueue(
            channel,
            "appointment.booked.queue",
            "appointment.booked");

        ConfigureQueue(
            channel,
            "appointment.confirmed.queue",
            "appointment.confirmed");

        ConfigureQueue(
            channel,
            "appointment.cancelled.queue",
            "appointment.cancelled");

        ConfigureQueue(
            channel,
            "consultation.completed.queue",
            "consultation.completed");
    }

    /// <summary>
    /// Declares a queue with DLX arguments and binds it to the main exchange.
    /// </summary>
    /// <param name="channel">RabbitMQ channel</param>
    /// <param name="queueName">Name of the queue to declare</param>
    /// <param name="routingKey">Routing key for binding to the main exchange</param>
    private static void ConfigureQueue(
        IModel channel,
        string queueName,
        string routingKey)
    {
        // Queue arguments: on failure, send to DLX with specific routing key
        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "appointment.dlx" },
            { "x-dead-letter-routing-key", "deadletter" }
        };

        // Declare queue (idempotent if it already exists)
        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);

        // Bind queue to main exchange
        channel.QueueBind(
            queue: queueName,
            exchange: "appointment.exchange",
            routingKey: routingKey);
    }
}