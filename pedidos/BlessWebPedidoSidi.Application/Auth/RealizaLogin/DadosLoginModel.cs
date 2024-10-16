namespace BlessWebPedidoSidi.Application.Auth.RealizaLogin;

public record DadosLoginModel
{
    public required string Token { get; set; }
    public required string TokenRefresh { get; set; }
    public required DateTime TokenDataValidade { get; set; }
}
