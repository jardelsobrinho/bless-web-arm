using System.ComponentModel.DataAnnotations;

namespace BlessWebPedidoSidi.Api.Models.Auths;

public record RealizaLoginPorUuidRequest
{
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string Uuid { get; set; }

}
