namespace BlessWebPedidoSidi.Api.Models.Modelo;

public record ModeloPesquisaRequest : PaginacaoRequest
{
    public int TabelaPrecoCodigo { get; set; }
    public int CondicaoPagamentoCodigo { get; set; }
    public string? ReferenciaOuDescricao { get; set; }

}
