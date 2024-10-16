namespace BlessWebPedidoSidi.Application.Marcas;

public record MarcaModel
{
    public required int Codigo { get; set; }
    public required string Nome { get; set; } = "";
}
