namespace BlessWebPedidoSidi.Application.Modelos.RetornaModelo.Models;

public record ModeloCorModel
{
    public int Codigo { get; set; }
    public required string Nome { get; set; }
    public required string Imagem { get; set; }
    public required double Preco { get; set; }
    public int? MarcaCodigo { get; set; }
    public int? SoladoCodigo { get; set; }
    public int? PalmilhaCodigo { get; set; }
    public string? ObservacaoItem { get; set; }
    public List<TamanhoModel> Grade { get; set; } = [];
}