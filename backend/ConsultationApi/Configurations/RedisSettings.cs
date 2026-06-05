namespace ConsultationApi.Configurations;

public class RedisSettings
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = default!;
    public string InstanceName { get; set; } = default!;
}