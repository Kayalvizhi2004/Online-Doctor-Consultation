namespace ConsultationWorker.Configurations;

public class RabbitMqSettings
{
    public string HostName { get; set; } = default!;

    public int Port { get; set; }

    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ExchangeName { get; set; } = default!;

    public string ExchangeType { get; set; } = default!;

    public string AppointmentQueue { get; set; } = default!;

    public string DeadLetterQueue { get; set; } = default!;
}