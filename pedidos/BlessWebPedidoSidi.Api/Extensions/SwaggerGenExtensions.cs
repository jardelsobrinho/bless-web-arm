using Microsoft.OpenApi.Models;

using System.Reflection;

namespace BlessWebPedidoSidi.Api.Extensions;

public static class SwaggerGenExtensions
{
    public static IServiceCollection AddSwaggerCustom(this IServiceCollection services)
    {
        return services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "BLESS WEB PEDIDO SIDI", Version = "v1" });

            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Entre com a chave da api",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}
