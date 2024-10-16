using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.Usuario;

public class UsuarioEntity : IEntity
{
    public required int Codigo { get; set; }
    public required string Usuario { get; set; }
    public required PasswordValueObject Password { get; set; }
    public required string NomeCompleto { get; set; }
    public string? CnpjRepresentante { get; set; }
}