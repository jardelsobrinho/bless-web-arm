using BlessWebPedidoSidi.Application.Modelos.PesquisaModelos;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaPrecos;

public class AtualizaPrecosHandler(IMediator mediator) : IRequestHandler<AtualizaPrecosCommand, OrcamentoWebEntity>
{
    public async Task<OrcamentoWebEntity> Handle(AtualizaPrecosCommand command, CancellationToken cancellationToken)
    {
        foreach (var item in command.OrcamentoWeb.Itens)
        {
            var precoModelo = 0.0;
            var tabelaPrecoCodigo = command.OrcamentoWeb.TabelaPrecoCodigo ?? 0;
            var condicaoPagamentoCodigo = command.OrcamentoWeb.CondicaoPagamentoCodigo ?? 0;

            if (tabelaPrecoCodigo != 0 && condicaoPagamentoCodigo != 0)
            {
                var queryModelo = new PesquisaModeloQuery
                {
                    TabelaPrecoCodigo = command.OrcamentoWeb.TabelaPrecoCodigo ?? 0,
                    CondicaoPagamentoCodigo = command.OrcamentoWeb.CondicaoPagamentoCodigo ?? 0,
                    ModeloCodigo = item.ModeloCodigo,
                    ReferenciaOuDescricao = "",
                    RepresentanteCnpj = command.OrcamentoWeb.RepresentanteCnpj,
                    Pagina = 1,
                    RegistrosPorPagina = 1
                };

                var modeloPesquisaModel = await mediator.Send(queryModelo, cancellationToken);
                if (modeloPesquisaModel.Registros.Count > 0)
                    precoModelo = modeloPesquisaModel.Registros[0].Preco;
            }
            item.PrecoUnitario = precoModelo;
        }

        var valorTotal = command.OrcamentoWeb.Itens.Sum(x => x.PrecoUnitario * x.TotalPares);
        command.OrcamentoWeb.ValorTotal = valorTotal;
        return command.OrcamentoWeb;
    }
}

public record AtualizaPrecosCommand : IRequest<OrcamentoWebEntity>
{
    public required OrcamentoWebEntity OrcamentoWeb { get; set; }
}