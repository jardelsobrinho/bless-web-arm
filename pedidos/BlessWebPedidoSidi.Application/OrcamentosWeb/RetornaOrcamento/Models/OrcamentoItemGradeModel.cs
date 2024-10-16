namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;

public record OrcamentoItemGradeModel
{
    public required int Id { get; set; }
    public required string TamanhoCodigo { get; set; }
    public required string TamanhoDescricao { get; set; }
    public required double Quantidade { get; set; }
    public required double Estoque { get; set; }
    public int Ordem { get; set; } = 0;
}