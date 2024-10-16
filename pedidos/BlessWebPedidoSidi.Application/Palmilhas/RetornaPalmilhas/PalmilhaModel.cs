namespace BlessWebPedidoSidi.Application.Palmilhas.RetornaPalmilhas;

public record PalmilhaModel
{
    public int Codigo { get; init; }
    public required string Descricao { get; init; }
    public required string Cor { get; init; }
    public required string DescricaoCor { get; init; }
}
