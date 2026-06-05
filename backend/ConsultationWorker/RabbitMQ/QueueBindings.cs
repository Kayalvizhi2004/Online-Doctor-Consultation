namespace ConsultationWorker.RabbitMQ;

public static class QueueBindings
{
    public static void Configure(dynamic channel)
    {
        ConfigureQueue(channel, "appointment.booked.queue", "appointment.booked");
        ConfigureQueue(channel, "appointment.confirmed.queue", "appointment.confirmed");
        ConfigureQueue(channel, "appointment.cancelled.queue", "appointment.cancelled");
        ConfigureQueue(channel, "consultation.completed.queue", "consultation.completed");
    }

    private static void ConfigureQueue(dynamic channel, string queueName, string routingKey)
    {
        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "appointment.dlx" },
            { "x-dead-letter-routing-key", "deadletter" }
        };

        // Sync methods for RabbitMQ.Client 6.8.1
        channel.QueueDeclare(queueName, true, false, false, arguments);
        channel.QueueBind(queueName, "appointment.exchange", routingKey, null);
    }
}