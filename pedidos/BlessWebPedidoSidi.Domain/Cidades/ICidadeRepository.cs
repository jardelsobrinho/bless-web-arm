using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.Cidades;

public interface ICidadeRepository : IRepository<CidadeEntity>
{
    Task<int> RetornaCodigoClienteAsync(int codigoIbge, string nomeCidade);
}