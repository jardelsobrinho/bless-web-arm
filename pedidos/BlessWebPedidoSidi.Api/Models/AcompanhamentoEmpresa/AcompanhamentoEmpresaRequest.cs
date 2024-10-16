namespace BlessWebPedidoSidi.Api.Models.AcompanhamentoEmpresa;

public record AcompanhamentoEmpresaRequest
{
    public DateTime? DataInicial { get; init; }
    public DateTime? DataFinal { get; init; }
}
