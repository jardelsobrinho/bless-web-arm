namespace BlessWebPedidoSidi.Api.Models.Auths;

public record AtualizaTokenRequest
{
    public required string TokenRefresh { get; set; }
}
