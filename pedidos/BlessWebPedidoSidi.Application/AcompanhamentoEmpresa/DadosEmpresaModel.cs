namespace BlessWebPedidoSidi.Application.AcompanhamentoEmpresa;

public record DadosEmpresaModel
{
    public required string Empresa { get; init; }
    public required string Cnpj { get; init; }
    public required int QuantidadePedido { get; init; }
}
