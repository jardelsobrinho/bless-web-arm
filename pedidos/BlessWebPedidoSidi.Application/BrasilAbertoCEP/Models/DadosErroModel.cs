namespace BlessWebPedidoSidi.Application.BrasilAbertoCEP.Models;

public record class DadosErroModel
{
    public int Code { get; init; }
    public required string Message { get; init; }

}
