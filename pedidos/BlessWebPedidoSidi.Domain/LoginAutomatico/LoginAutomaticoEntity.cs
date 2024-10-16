using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.LoginAutomatico;

public class LoginAutomaticoEntity : IEntity
{
    public required int Id { get; set; }
    public required string Uuid { get; set; }
    public required string Token { get; set; }
    public required string UrlApi { get; set; }
    public required string Usuario { get; set; }
    public required DateTime DataValidade { get; set; }
}