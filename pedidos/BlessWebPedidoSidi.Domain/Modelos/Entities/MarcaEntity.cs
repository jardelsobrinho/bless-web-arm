using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.Modelos.Entities;

public class MarcaEntity : IEntity
{
    public required int Codigo { get; set; }
    public required string Nome { get; set; }
}