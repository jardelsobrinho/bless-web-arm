namespace BlessWebPedidoSidi.Application.BrasilAbertoCEP.Models;

public record DadosResultCEPModel<T>
{
    public required T Result { get; init; }
}
