using System.ComponentModel.DataAnnotations;

namespace BlessWebPedidoSidi.Api.Models.ClientesWeb;

public record ClienteWebRequest
{
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required int Id { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    [Display(Name = "CNPJ/CPF")]
    public required string CnpjCpf { get; set; }

    public string? InscricaoEstadual { get; set; }
    public string? TipoInscricaoEstadual { get; set; }

    [Display(Name = "Razão Social")]
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? TelefoneDDD { get; set; }
    public string? TelefoneNumero { get; set; }

    [Display(Name = "DDD Celular")]
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string CelularDDD { get; set; }

    [Display(Name = "Número Celular")]
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string CelularNumero { get; set; }

    [Display(Name = "Nome do Contato")]
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string ContatoNome { get; set; }

    [Display(Name = "Email do Contato")]
    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string ContatoEmail { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required IList<ClienteWebEnderecoRequest> Enderecos { get; set; }

    [Required]
    public required bool EnderecoEntregaIgualPrincipal { get; set; }
    [Required]
    public required bool EnderecoCobrancaIgualPrincipal { get; set; }
    public IList<int>? EnderecosIdsExcluidos { get; set; }

    [Required(ErrorMessage = "O campo {0} deve ser preenchido")]
    public required string FretePorConta { get; set; }
}
