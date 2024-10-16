namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb.AdicionaModelo;

public record AdicionaModeloRequest
{
    public required int ModeloCodigo { get; set; }
    public required IList<CorRequest> Cores { get; set; }
    public required bool SomaQuantidade { get; set; }
}
