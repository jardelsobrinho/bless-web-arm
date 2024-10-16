namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;

public record OrcamentoModel
{
    public required DateTime DataCriacao { get; set; }
    public required double ValorTotal { get; set; }
    public required string Uuid { get; set; }
    public DateTime? DataEmissao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? ClienteCnpjCpf { get; set; }
    public int? TabelaPrecoCodigo { get; set; }
    public string? TabelaPrecoNome { get; set; }
    public int? CondicaoPagamentoCodigo { get; set; }
    public string? CondicaoPagamentoDescricao { get; set; }
    public string? TipoEstoqueCodigo { get; set; }
    public string? Frete { get; set; }
    public string? ClienteNome { get; set; }
    public required string Status { get; set; }
    public required bool ValidaEstoque { get; set; }
    public required bool ExibirObservacaoItem { get; set; }
    public string? Obs { get; set; }
    public IList<OrcamentoItemModel> Itens { get; set; } = [];
}