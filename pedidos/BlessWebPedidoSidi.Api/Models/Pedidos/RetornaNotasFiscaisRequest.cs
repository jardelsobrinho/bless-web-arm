namespace BlessWebPedidoSidi.Api.Models.Pedidos;

public record RetornaNotasFiscaisRequest
{
    public int Pedido { get; init; }
    public int Sequencia { get; init; }
}
