namespace BlessWebPedidoSidi.Application.Cidades;

public record CidadeModel
{
    public int Codigo { get; set; }
    public required string Nome { get; set; }
    public required int CodigoIbge { get; set; }
}