using Asp.Versioning;

namespace BlessWebPedidoSidi.Api.Extensions;

public static class SwaggerVersioningExtension
{
    public static void AddSwaggerVersioningExtension(this IServiceCollection service)
    {
        service.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
        }).AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
    }
}
