using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.LoginAutomatico;

public interface IControleSistemaRepository : IRepository<ControleSistemaEntity>
{
    Task<ControleSistemaEntity> RetornaAsync();
}
