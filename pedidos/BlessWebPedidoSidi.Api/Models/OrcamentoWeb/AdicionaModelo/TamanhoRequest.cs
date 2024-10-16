namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb.AdicionaModelo;

public record TamanhoRequest
{
    public required string TamanhoCodigo { get; set; }
    public required int Quantidade { get; set; }
}
