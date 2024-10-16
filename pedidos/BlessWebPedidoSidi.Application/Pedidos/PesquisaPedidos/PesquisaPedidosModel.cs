namespace BlessWebPedidoSidi.Application.Pedidos.PesquisaPedidos;

public record PesquisaPedidosModel
{
    public required string OrcamentoUuid { get; set; }
    public required string ClienteCnpjCpf { get; set; }
    public required string ClienteNome { get; set; }
    public required DateTime DataEmissao { get; set; }
    public required DateTime DataEntrega { get; set; }
    public required double ValorTotal { get; set; }
    public required string Status { get; set; }
    public required string Numero { get; set; }
    public required string Situacao { get; set; }
    public required string Online { get; set; }
}
