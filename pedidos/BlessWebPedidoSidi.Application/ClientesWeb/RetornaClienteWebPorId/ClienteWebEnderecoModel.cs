namespace BlessWebPedidoSidi.Application.ClientesWeb.RetornaClienteWebPorId;

public record ClienteWebEnderecoModel
{
    public required int Id { get; set; }
    public required string Rua { get; set; }
    public required string Numero { get; set; }
    public required string Complemento { get; set; }
    public required string Bairro { get; set; }
    public required string Cep { get; set; }
    public required int CidadeId { get; set; }
    public required string UfSigla { get; set; }
    public required string Tipo { get; set; }
}