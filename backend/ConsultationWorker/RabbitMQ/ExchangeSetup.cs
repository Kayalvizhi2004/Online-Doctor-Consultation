namespace ConsultationWorker.RabbitMQ;

public static class ExchangeSetup
{
    public static void Configure(dynamic channel)
    {
        // Sync methods for RabbitMQ.Client 6.8.1
        channel.ExchangeDeclare("appointment.exchange", "direct", true, false, null);
    }
}