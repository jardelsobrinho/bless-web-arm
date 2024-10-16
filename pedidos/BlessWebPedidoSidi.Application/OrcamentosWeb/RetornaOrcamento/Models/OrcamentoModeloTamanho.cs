namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;

public record OrcamentoModeloTamanho
{

    public required string Codigo { get; init; }
    public required string Descricao { get; init; }
    public required int Ordem { get; init; }

}
