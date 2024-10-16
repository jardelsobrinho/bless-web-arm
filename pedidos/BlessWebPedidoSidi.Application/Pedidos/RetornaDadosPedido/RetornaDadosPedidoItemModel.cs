namespace BlessWebPedidoSidi.Application.Pedidos.RetornaDadosPedido;

public record RetornaDadosPedidoItemModel
{
    public required int Modelo { get; set; }
    public required int Versao { get; set; }
    public required string Referencia { get; set; }
    public required string ModeloDescricao { get; set; }
    public required string CorNome { get; set; }
    public required double TotalPares { get; set; }
    public required double PrecoUnitario { get; set; }
    public int? MarcaCodigo { get; set; }
    public string? MarcaNome { get; set; }
    public string Imagem { get; set; } = string.Empty;
    public int Sequencia { get; set; }
}
