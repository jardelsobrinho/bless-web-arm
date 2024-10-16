using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.ClientesWeb.Entities;

public class ClienteWebEntity : IEntity
{
    public int Id { get; set; }
    public required string CnpjCpf { get; set; }
    public string? InscricaoEstadual { get; set; }
    public ETipoInscricaoEstadual? TipoInscricaoEstadual { get; set; }
    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public string? TelefoneDDD { get; set; }
    public string? TelefoneNumero { get; set; }
    public required string CelularDDD { get; set; }
    public required string CelularNumero { get; set; }
    public required string ContatoNome { get; set; }
    public required string ContatoEmail { get; set; }
    public required string EnderecoEntregaIgualPrincipal { get; set; }
    public required string EnderecoCobrancaIgualPrincipal { get; set; }
    public required string FretePorConta { get; set; }
    public required string Sincronizado { get; set; }
    public ICollection<ClienteWebEnderecoEntity> Enderecos { get; set; } = new List<ClienteWebEnderecoEntity>();
}