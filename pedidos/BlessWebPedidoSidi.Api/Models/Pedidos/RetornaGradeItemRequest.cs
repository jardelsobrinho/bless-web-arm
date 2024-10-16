namespace BlessWebPedidoSidi.Api.Models.Pedidos;

public record RetornaGradeItemRequest
{
    public int Pedido { get; init; }
    public int Sequencia { get; init; }
}
