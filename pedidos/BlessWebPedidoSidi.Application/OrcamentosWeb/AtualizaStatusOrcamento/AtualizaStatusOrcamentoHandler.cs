using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaStatusOrcamento;

public class AtualizaStatusOrcamentoHandler(IMediator mediator) : IRequestHandler<AtualizaStatusOrcamentoCommand, OrcamentoWebEntity>
{
    public async Task<OrcamentoWebEntity> Handle(AtualizaStatusOrcamentoCommand command, CancellationToken cancellationToken)
    {
        command.Orcamento.Status = command.Status;
        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = command.Orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);
        return command.Orcamento;
    }
}

public record AtualizaStatusOrcamentoCommand : IRequest<OrcamentoWebEntity>
{
    public required OrcamentoWebEntity Orcamento { get; set; }
    public required EOrcamentoStatus Status { get; set; }
}