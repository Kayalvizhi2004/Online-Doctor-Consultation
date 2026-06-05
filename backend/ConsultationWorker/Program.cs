using ConsultationWorker;
using ConsultationWorker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Register worker services (DI, RabbitMQ settings, hosted consumer)
// Explicitly call the extension in the root `ConsultationWorker` namespace to avoid ambiguity
ConsultationWorker.ServiceCollectionExtensions.AddWorkerServices(builder.Services, builder.Configuration);

var host = builder.Build();
host.Run();
