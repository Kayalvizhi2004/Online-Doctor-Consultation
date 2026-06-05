namespace ConsultationApi.Configurations;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ExchangeName { get; set; } = default!;
    public string ExchangeType { get; set; } = default!;
}