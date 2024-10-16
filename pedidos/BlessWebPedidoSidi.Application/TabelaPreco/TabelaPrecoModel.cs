namespace BlessWebPedidoSidi.Application.TabelaPreco;

public record TabelaPrecoModel
{
    public int Codigo { get; set; }
    public required string Nome { get; set; }
}