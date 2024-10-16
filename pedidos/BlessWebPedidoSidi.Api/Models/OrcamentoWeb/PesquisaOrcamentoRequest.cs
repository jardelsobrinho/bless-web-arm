namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb;

public record PesquisaOrcamentoRequest
{
    public required string Status { get; set; }
    public string? PedidoNumeroOuClienteNome { get; set; }
    public DateTime? PedidoDataEmissao { get; set; }
}
