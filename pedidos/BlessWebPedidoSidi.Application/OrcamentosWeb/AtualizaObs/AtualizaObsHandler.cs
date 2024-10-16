using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaObs;

public class AtualizaObsHandler(IMediator mediator) : IRequestHandler<AtualizaObsCommand, Unit>
{
    public async Task<Unit> Handle(AtualizaObsCommand command, CancellationToken cancellationToken)
    {
        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        orcamento.Obs = command.Obs;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }
}

public record AtualizaObsCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required string Obs { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}
