namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb;

public record AtualizaCondPagtoOrcamentoRequest
{
    public required int CondicaoPagamentoCodigo { get; set; }
    public required int TabelaPrecoCodigo { get; set; }
}
