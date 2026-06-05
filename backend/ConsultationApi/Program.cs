using ConsultationApi.Extensions;
using ConsultationApi.Hubs;
using ConsultationApi.Middleware;
using ConsultationApi.Application;
using ConsultationApi.Infrastructure;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

#region Controllers

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();

#endregion

#region Swagger

builder.Services.AddSwaggerDocs();

#endregion

#region Application + Infrastructure DI

builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

#endregion

#region Database

// Database registration is handled in the Infrastructure DI

#endregion

#region JWT Authentication

builder.Services.AddJwtAuthentication(configuration);

#endregion

#region Authorization

builder.Services.AddAuthorization();

#endregion

#region SignalR

builder.Services.AddSignalRServices();

#endregion

#region CORS

builder.Services.AddCorsPolicy();

#endregion

#region HttpContextAccessor

builder.Services.AddHttpContextAccessor();

#endregion

var app = builder.Build();

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Hide the Models/Schemas panel (removes "Example Value" and "Schema")
        c.DefaultModelsExpandDepth(-1);
        c.DefaultModelExpandDepth(-1);
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseAuthentication();

app.UseAuthorization();

#endregion

#region Controllers

app.MapControllers();

#endregion

#region SignalR

app.MapHub<ConsultationHub>(
    "/hubs/consultation");

#endregion

#region Run

app.Run();

#endregion