namespace BlessWebPedidoSidi.Application.Modelos.RetornaPreco;

public record RetornaPrecoModel
{
    public required bool ProdutoExisteNaTabelaPreco { get; init; }
    public double Preco { get; init; } = 0;
}
