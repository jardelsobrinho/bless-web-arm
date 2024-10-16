namespace BlessWebPedidoSidi.Application.OrcamentosWeb.PesquisaOrcamentos;

public record PesquisaOrcamentoModel
{
    public required int Id { get; set; }
    public required string Uuid { get; set; }
    public required string ClienteCnpjCpf { get; set; }
    public required string ClienteNome { get; set; }
    public required DateTime DataCriacao { get; set; }
    public required DateTime DataEntrega { get; set; }
    public required double ValorTotal { get; set; }
    public required string Status { get; set; }
    public string? NumeroPedidoErp { get; set; }
    public string? SituacaoPedidoErp { get; set; }
}