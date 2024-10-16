namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;

public record TamanhoModel
{
    public required string TamanhoCodigo { get; set; }
    public required int Quantidade { get; set; }
}