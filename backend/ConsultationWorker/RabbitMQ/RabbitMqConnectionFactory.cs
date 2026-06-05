using System.Reflection;
using Microsoft.Extensions.Options;
using ConsultationWorker.Configurations;
using RabbitMQ.Client;

namespace ConsultationWorker.RabbitMQ;

public class RabbitMqConnectionFactory
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqConnectionFactory(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
    }

    public dynamic CreateConnection()
    {
        // Create ConnectionFactory via reflection to avoid compile-time
        // dependency on RabbitMQ.Client types. The RabbitMQ.Client
        // assembly should be available at runtime for this to succeed.
        var factoryType = Type.GetType("RabbitMQ.Client.ConnectionFactory, RabbitMQ.Client");
        if (factoryType == null)
            throw new InvalidOperationException("RabbitMQ.Client.ConnectionFactory type not found at runtime.");

        var factory = Activator.CreateInstance(factoryType)!;

        factoryType.GetProperty("HostName")?.SetValue(factory, _settings.HostName);
        factoryType.GetProperty("Port")?.SetValue(factory, _settings.Port);
        factoryType.GetProperty("UserName")?.SetValue(factory, _settings.UserName);
        factoryType.GetProperty("Password")?.SetValue(factory, _settings.Password);

        var dispatchProp = factoryType.GetProperty("DispatchConsumersAsync");
        if (dispatchProp != null && dispatchProp.CanWrite)
            dispatchProp.SetValue(factory, true);

        var createConn = factoryType.GetMethod("CreateConnection", Type.EmptyTypes);
        if (createConn == null)
            throw new InvalidOperationException("CreateConnection method not found on ConnectionFactory.");

        var connection = createConn.Invoke(factory, null);
        return connection!;
    }
}