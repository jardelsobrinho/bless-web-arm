namespace BlessWebPedidoSidi.Application.Modelos.PesquisaModelos;

public record ModeloPesquisaModel
{
    public int Codigo { get; set; }
    public required int ReferenciaCodigo { get; set; }
    public required string Referencia { get; set; }
    public required string Descricao { get; set; }
    public required double Preco { get; set; } = 0;
    public required double PrecoMenor { get; set; } = 0;
    public required double PrecoMaior { get; set; } = 0;
    public required string TamanhoInicial { get; set; }
    public required string TamanhoFinal { get; set; }
    public required string Imagem { get; set; }
    public required IList<ModeloCoresPesquisaModel> OutrasCores { get; set; } = [];

}