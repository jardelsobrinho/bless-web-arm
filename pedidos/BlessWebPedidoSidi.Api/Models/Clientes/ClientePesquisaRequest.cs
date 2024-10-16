namespace BlessWebPedidoSidi.Api.Models.Clientes;

public record ClientePesquisaRequest: PaginacaoRequest
{
    public string? CnpjOuNome { get; set; }
}
