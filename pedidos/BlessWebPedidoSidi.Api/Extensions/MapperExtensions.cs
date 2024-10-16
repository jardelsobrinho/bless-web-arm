using AutoMapper;
using BlessWebPedidoSidi.Api.Extensions.Mapping;

namespace BlessWebPedidoSidi.Api.Extensions;

public static class MapperExtensions
{
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper(); 
        return services.AddSingleton(mapper);
    }
}
