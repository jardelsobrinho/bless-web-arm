using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaTipoEstoque;

public class AtualizaTipoEstoqueHandler(IMediator mediator) : IRequestHandler<AtualizaTipoEstoqueCommand, Unit>
{
    public async Task<Unit> Handle(AtualizaTipoEstoqueCommand command, CancellationToken cancellationToken)
    {
        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        orcamento.TipoEstoqueCodigo = command.TipoEstoqueCodigo;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }
}

public record AtualizaTipoEstoqueCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required string TipoEstoqueCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}