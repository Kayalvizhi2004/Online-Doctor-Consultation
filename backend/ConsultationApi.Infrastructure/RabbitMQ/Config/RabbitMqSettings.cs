namespace ConsultationApi.Infrastructure.RabbitMQ.Config;

public class RabbitMqSettings
{
    public string HostName { get; set; }
        = "localhost";

    public string UserName { get; set; }
        = "guest";

    public string Password { get; set; }
        = "guest";

    public string ExchangeName { get; set; }
        = "appointment.exchange";

    public string DeadLetterExchange { get; set; }
        = "appointment.dlx";
}