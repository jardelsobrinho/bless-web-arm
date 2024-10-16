namespace BlessWebPedidoSidi.Application.EstoqueAcabado;

public record EstoqueAcabadoModel
{
    public required int ModeloCodigo { get; set; }
    public required int CorCodigo { get; set; }
    public required string TamanhoCodigo { get; set; }
    public required double Estoque { get; set; }
}