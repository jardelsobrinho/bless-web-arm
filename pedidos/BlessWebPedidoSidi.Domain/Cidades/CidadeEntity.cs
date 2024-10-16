using BlessWebPedidoSidi.Domain.Shared;
using BlessWebPedidoSidi.Domain.Ufs;

namespace BlessWebPedidoSidi.Domain.Cidades;

public class CidadeEntity : IEntity
{
    public required int Codigo { get; set; }
    public required string Nome { get; set; }
    public required string UfSigla { get; set; }
    public required int CodigoIbge { get; set; }
    public required UfEntity Uf { get; set; }
}