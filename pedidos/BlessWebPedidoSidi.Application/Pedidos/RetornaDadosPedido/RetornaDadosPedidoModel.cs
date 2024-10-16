namespace BlessWebPedidoSidi.Application.Pedidos.RetornaDadosPedido;

public record RetornaDadosPedidoModel
{
    public required int Numero { get; set; }
    public required string DataEmissao { get; set; }
    public required string Situacao { get; set; }
    public required string ClienteRazaoSocial { get; set; }
    public required string ClienteCnpjCpf { get; set; }
    public required string TabelaPrecoNome { get; set; }
    public required string CondicaoPagamentoDescricao { get; set; }
    public required double ValorTotal { get; set; }
    public IList<RetornaDadosPedidoItemModel> Itens { get; set; } = new List<RetornaDadosPedidoItemModel>();
}
