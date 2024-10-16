namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb.AdicionaModelo;

public record CorRequest
{
    public required int CorCodigo { get; init; }
    public int QuantCaixas { get; init; } = 0;
    public required IList<TamanhoRequest> Grade { get; init; }
    public required int MarcaCodigo { get; init; }
    public int? SoladoCodigo { get; init; }
    public int? PalmilhaCodigo { get; init; }
    public string? ObservacaoItem { get; init; }
    public int? Id { get; set; }
}
