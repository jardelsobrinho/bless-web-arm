using BlessWebPedidoSidi.Domain.Modelos.Entities;
using BlessWebPedidoSidi.Domain.Shared;


namespace BlessWebPedidoSidi.Domain.Modelos.Repositories;

public interface IMarcaRepository : IRepository<MarcaEntity>
{
    Task<string> RetornaNomeMarca(int codigo);
}
