using BlessWebPedidoSidi.Application.Marcas;
using BlessWebPedidoSidi.Application.Palmilhas.RetornaPalmilhas;
using BlessWebPedidoSidi.Application.Solados.RetornaSolados;

namespace BlessWebPedidoSidi.Api.Models.Modelo;

public record ModeloDadosCadastroResponse
{
    public required IList<MarcaModel> Marcas { get; init; }
    public required IList<SoladoModel> Solados { get; init; }
    public required IList<PalmilhaModel> Palmilhas { get; init; }
}
