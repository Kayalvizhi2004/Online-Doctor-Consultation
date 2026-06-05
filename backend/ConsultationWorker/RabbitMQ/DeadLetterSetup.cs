namespace ConsultationWorker.RabbitMQ;

public static class DeadLetterSetup
{
    public static void Configure(dynamic channel)
    {
        // Sync methods for RabbitMQ.Client 6.8.1
        channel.ExchangeDeclare("appointment.dlx", "direct", true, false, null);
        channel.QueueDeclare("appointment.dlq", true, false, false, null);
        channel.QueueBind("appointment.dlq", "appointment.dlx", "deadletter", null);
    }
}