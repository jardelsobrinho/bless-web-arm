using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.Modelos.Entities;

public class CorEntity : IEntity
{
    public required int Codigo { get; set; }
    public required int ModeloCodigo { get; set; }
    public required string Nome { get; set; }
}