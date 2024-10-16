namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb;

public record AtualizaClienteOrcamentoRequest
{
    public required string ClienteCnpjCpf { get; set; }
}
