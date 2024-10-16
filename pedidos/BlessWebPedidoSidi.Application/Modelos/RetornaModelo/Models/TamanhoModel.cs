namespace BlessWebPedidoSidi.Application.Modelos.RetornaModelo.Models;

public record TamanhoModel
{
    public required string Codigo { get; set; }
    public required string Descricao { get; set; }
    public required double Estoque { get; set; }
}