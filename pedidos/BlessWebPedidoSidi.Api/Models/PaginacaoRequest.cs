namespace BlessWebPedidoSidi.Api.Models;

public record PaginacaoRequest
{
    public required int Pagina { get; set; } = 1;
    public required int RegistrosPorPagina { get; set; } = 12;
}
