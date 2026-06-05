using ConsultationApi.Infrastructure.RabbitMQ.Config;

namespace ConsultationApi.Infrastructure.RabbitMQ.Setup;

public static class ExchangeSetup
{
    public static void Configure(RabbitMqSettings settings)
    {
        // No-op in this build: exchange declaration performed by the Worker
        // at runtime. Keeping this method avoids compile-time dependency on
        // the RabbitMQ.Client types in the API project.
    }
}