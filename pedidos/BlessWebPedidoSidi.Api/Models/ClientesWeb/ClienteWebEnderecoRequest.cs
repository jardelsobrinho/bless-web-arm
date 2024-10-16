using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace BlessWebPedidoSidi.Api.Models.ClientesWeb;

public record ClienteWebEnderecoRequest
{
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string Rua { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string Numero { get; set; }

    public required string Complemento { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string Bairro { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string Cep { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public int CidadeId { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    [EnumDataType(typeof(ETipoEndereco), ErrorMessage = "O {0} deve ser Principal, Entrega ou Cobranca")]
    public required string Tipo { get; set; }
}
