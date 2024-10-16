namespace BlessWebPedidoSidi.Application.Pedidos.RetornaGradeItem;

public record RetornaGradeItemConsulta
{
    public String? NOTA_FISCAL { get; init; }
    public int SEQ_ITEM { get; init; }
    public required string TAMANHO_CALCADO { get; init; }
    public int QTE_PEDIDA { get; init; }
    public int MODELO {  get; init; }
}
