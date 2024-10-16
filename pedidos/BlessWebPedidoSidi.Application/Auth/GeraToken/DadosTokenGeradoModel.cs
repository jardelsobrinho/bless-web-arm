namespace BlessWebPedidoSidi.Application.Auth.GeraToken;

public record DadosTokenGeradoModel
{
    public required string Token { get; set; }
    public required DateTime TokenDataValidade { get; set; }
}
