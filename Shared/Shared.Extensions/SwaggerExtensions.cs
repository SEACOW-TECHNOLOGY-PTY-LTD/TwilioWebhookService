using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Shared.Extensions;

public static class SwaggerExtensions
{
    public static void AddCustomSwagger(this IServiceCollection services, string apiTitle, string apiVersion)
    {
        services.AddSwaggerGen(ctx =>
        {
            ctx.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Title = apiTitle,
                Version = apiVersion,
                Description = $"{apiTitle} Services.",
                Contact = new OpenApiContact
                {
                    Name = "Ran Tao",
                    Email = "cs.pangge@gmail.com"
                },
            });
            ctx.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            ctx.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
                
            ctx.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}