namespace BlessWebPedidoSidi.Api.Models.Modelo;

public record ModeloRetornoRequest
{
    public int? TabelaPrecoCodigo { get; set; }
    public int? CondicaoPagamentoCodigo { get; set; }
    public string? OrcamentoUuid { get; set; }
}
