namespace BlessWebPedidoSidi.Api.Models.Pedidos;

public record PesquisaPedidoRequest : PaginacaoRequest
{
    public string? NumeroOuNomeCliente { get; set; }
    public DateTime? DataEmissao { get; set; }
}
