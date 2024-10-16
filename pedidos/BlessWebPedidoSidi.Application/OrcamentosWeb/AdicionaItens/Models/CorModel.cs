namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;

public record CorModel
{
    public int? Id { get; init; }
    public required int CorCodigo { get; init; }
    public required int QuantCaixas { get; init; } = 0;
    public required int MarcaCodigo { get; init; }
    public int? SoladoCodigo { get; init; }
    public int? PalmilhaCodigo { get; init; }
    public string? ObservacaoItem { get; init; }
    public required IList<TamanhoModel> Grade { get; init; }
}