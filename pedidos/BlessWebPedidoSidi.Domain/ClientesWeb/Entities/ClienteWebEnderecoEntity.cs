using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.ClientesWeb.Entities;

public class ClienteWebEnderecoEntity : IEntity
{
    public int Id { get; set; }
    public required int ClienteId { get; set; }
    public required string Rua { get; set; }
    public required string Numero { get; set; }
    public required string Complemento { get; set; }
    public required string Bairro { get; set; }
    public required string Cep { get; set; }
    public required int CidadeCodigo { get; set; }
    public required ETipoEndereco Tipo { get; set; }
    public required ClienteWebEntity ClienteWeb { get; set; }
    public required CidadeEntity Cidade { get; set; }
}