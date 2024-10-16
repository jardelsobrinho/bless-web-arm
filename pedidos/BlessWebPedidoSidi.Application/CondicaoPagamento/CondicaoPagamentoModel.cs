namespace BlessWebPedidoSidi.Application.CondicaoPagamento;

public record CondicaoPagamentoModel
{
    public required int Codigo { get; set; }
    public required string Descricao { get; set; }
    public required int PrazoMedio { get; set; }
}