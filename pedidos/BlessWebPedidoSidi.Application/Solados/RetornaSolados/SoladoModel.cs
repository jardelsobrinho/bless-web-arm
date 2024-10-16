namespace BlessWebPedidoSidi.Application.Solados.RetornaSolados;

public record SoladoModel
{
    public int Codigo { get; init; }
    public required string Descricao { get; init; }
    public required string Cor { get; init; }
    public required string DescricaoCor { get; init; }
}
