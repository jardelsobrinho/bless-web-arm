namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;

public record PreferenciaModeloModel
{
    public required string ValidarGradePedido { get; init; }
    public required double QuantidadeMinima { get; init; }
}
