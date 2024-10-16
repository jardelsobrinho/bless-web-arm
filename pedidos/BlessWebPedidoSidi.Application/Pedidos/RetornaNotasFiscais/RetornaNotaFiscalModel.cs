namespace BlessWebPedidoSidi.Application.Pedidos.RetornaNotasFiscais;

public record RetornaNotaFiscalModel
{
    public int Entrega { get; set; }
    public int Numero { get; init; }
    public required string Serie { get; init; }
}
