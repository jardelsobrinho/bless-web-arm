namespace BlessWebPedidoSidi.Application.ClientesWeb.RetornaClienteWebPorId;

public record ClienteWebModel
{
    public int Id { get; set; }
    public required string CnpjCpf { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? TipoInscricaoEstadual { get; set; }
    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? TelefoneDDD { get; set; }
    public string? TelefoneNumero { get; set; }
    public required string CelularDDD { get; set; }
    public required string CelularNumero { get; set; }
    public required string ContatoNome { get; set; }
    public required string ContatoEmail { get; set; }
    public required bool EnderecoEntregaIgualPrincipal { get; set; }
    public required bool EnderecoCobrancaIgualPrincipal { get; set; }
    public required IList<ClienteWebEnderecoModel> Enderecos { get; set; }
    public required string FretePorConta { get; set; }
}