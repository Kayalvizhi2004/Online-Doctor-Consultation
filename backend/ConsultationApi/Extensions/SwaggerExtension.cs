using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

namespace ConsultationApi.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerDocs(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Consultation API",
                    Version = "v1"
                });

            // Include XML comments (controller and action summaries)
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        return services;
    }
}