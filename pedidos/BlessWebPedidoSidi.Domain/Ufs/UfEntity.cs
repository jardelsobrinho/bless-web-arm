using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.Ufs;

public class UfEntity : IEntity
{
    public required string Sigla { get; set; }
    public required string Descricao { get; set; }
    public required List<CidadeEntity> Cidades { get; set; }
}