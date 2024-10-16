using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.ControleSistema;

public class ControleSistemaEntity: IEntity
{
    public required string Cnpj { get; set; }
}