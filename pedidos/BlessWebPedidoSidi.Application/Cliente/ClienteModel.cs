using System.Diagnostics.CodeAnalysis;

namespace BlessWebPedidoSidi.Application.Cliente;

public record ClienteModel
{
    public required int ClienteId { get; set; }
    public required string CnpjCpf { get; set; }
    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? Endereco { get; set; }
    public string? Bairro { get; set; }
    public required string Cidade { get; set; }
    public required string UF { get; set; }
    public required string CEP { get; set; }
    public string? Telefones { get; set; }
    public string? ContatoEmail { get; set; }
    public required string Origem { get; set; }
    public string Bloqueado { get; set; } = "F";
    public required string FretePorConta { get; set; }
}