namespace BlessWebPedidoSidi.Application.Pedidos.RetornaDadosPedido;

public record RetornaTamanhosConsulta
{
    public required string TAMANHO_CALCADO { get; init; }
    public int QTE_PEDIDA { get; init; }
    public int MODELO { get; init; }
}
