namespace BlessWebPedidoSidi.Application.Modelos.RetornaModelo.Models;

public record ModeloModel
{
    public int Codigo { get; set; }
    public required int ReferenciaCodigo { get; set; }
    public required string Referencia { get; set; }
    public required string Descricao { get; set; }
    public required string TamanhoInicial { get; set; }
    public required string TamanhoFinal { get; set; }
    public required string Imagem { get; set; }
    public IList<ModeloCorModel> Cores { get; set; } = [];
    public required bool ValidaEstoque { get; set; }
    public required bool ExibirObservacaoItem { get; set; }
}