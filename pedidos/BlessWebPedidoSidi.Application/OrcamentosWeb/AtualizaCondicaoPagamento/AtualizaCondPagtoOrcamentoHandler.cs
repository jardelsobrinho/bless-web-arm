using BlessWebPedidoSidi.Application.CondicaoPagamento.PesquisaCondicaoPagamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaCondicaoPagamento;

public class AtualizaCondPagtoOrcamentoHandler(IMediator mediator) : IRequestHandler<AtualizaCondPagtoOrcamentoCommand, Unit>
{
    public async Task<Unit> Handle(AtualizaCondPagtoOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var pesquisaCondPagtoQuery = new PesquisaCondicaoPagamentoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            CondicaoPagamentoDescricao = "",
            UsuarioCodigo = command.UsuarioCodigo,
            TabelaPrecoCodigo = command.TabelaPrecoCodigo
        };

        var condPagtoHandler = await mediator.Send(pesquisaCondPagtoQuery, cancellationToken);
        if (condPagtoHandler.Count == 0)
            throw new BadHttpRequestException($"ACPH01 - Condição de pagamento não encontrada com o codigo {command.CondicaoPagamentoCodigo}");

        var condicaoPagamento = condPagtoHandler.Where(x => x.Codigo == command.CondicaoPagamentoCodigo).SingleOrDefault()
            ?? throw new BadHttpRequestException($"ACPH03 - Condição de pagamento não encontrada com o codigo {command.CondicaoPagamentoCodigo}");

        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        orcamento.CondicaoPagamentoCodigo = condicaoPagamento.Codigo;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }
}

public record AtualizaCondPagtoOrcamentoCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required int CondicaoPagamentoCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int TabelaPrecoCodigo { get; set; }
    public required int UsuarioCodigo { get; set; }
}