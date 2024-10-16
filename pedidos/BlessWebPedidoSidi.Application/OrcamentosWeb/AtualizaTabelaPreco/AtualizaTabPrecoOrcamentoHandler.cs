using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using BlessWebPedidoSidi.Application.TabelaPreco.PesquisaTabelaPreco;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaTabelaPreco;

public class AtualizaCondPagtoOrcamentoHandler(IMediator mediator) : IRequestHandler<AtualizaTabPrecoOrcamentoCommand, Unit>
{
    public async Task<Unit> Handle(AtualizaTabPrecoOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var pesquisaTabelaPrecoQuery = new PesquisaTabelaPrecoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            TabelaPrecoNome = "",
            TabelaPrecoCodigo = command.TabelaPrecoCodigo
        };

        var tabelaPrecoHandle = await mediator.Send(pesquisaTabelaPrecoQuery, cancellationToken);
        if (tabelaPrecoHandle.Count == 0)
            throw new BadHttpRequestException($"ATPOC01 - Tabela de preço não encontrada com o codigo {command.TabelaPrecoCodigo}");

        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        orcamento.TabelaPrecoCodigo = tabelaPrecoHandle.First().Codigo;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }
}

public record AtualizaTabPrecoOrcamentoCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required int TabelaPrecoCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}