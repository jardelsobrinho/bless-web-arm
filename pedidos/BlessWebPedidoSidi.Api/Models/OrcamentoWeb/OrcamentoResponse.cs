namespace BlessWebPedidoSidi.Api.Models.OrcamentoWeb;

public record OrcamentoResponse
{
    public required int Id { get; set; }
    public required string Uuid { get; set; }
    public required string ClienteCnpj { get; set; }
    public required string ClienteNome { get; set; }
    public required DateTime DataCriacao { get; set; }
}
