namespace BlessWebPedidoSidi.Application.Cliente.RetornaRepresentante;

public record RepresentanteModel
{
    public int Id { get; set; }
    public required string CnpjCpf { get; set; }
    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? TelefoneDDD { get; set; }
    public string? TelefoneNumero { get; set; }
    public required string CelularDDD { get; set; }
    public required string CelularNumero { get; set; }
    public required string ContatoNome { get; set; }
    public required string ContatoEmail { get; set; }
}
