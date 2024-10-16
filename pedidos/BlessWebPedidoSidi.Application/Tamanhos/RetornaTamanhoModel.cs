namespace BlessWebPedidoSidi.Application.Tamanhos;

public record RetornaTamanhoModel
{
    public required string Codigo { get; init; }
    public required string Descricao { get; init; }
    public int Ordem { get; init; }
}
