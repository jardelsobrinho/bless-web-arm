using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using BlessWebPedidoSidi.Domain.Shared;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.ExcluiOrcamento;

public class ExcluiOrcamentoHandler(IMediator mediator, IUnitOfWork unitOfWork) : IRequestHandler<ExcluiOrcamentoCommand, Unit>
{
    public async Task<Unit> Handle(ExcluiOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        try
        {
            await unitOfWork.StartTransactionAsync();
            await unitOfWork.OrcamentoWebRepository.ExcluirAsync(x => x.Id == orcamento.Id);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new Exception("EOH01 - Falha ao excluir o orçamento", ex);
        }

        return Unit.Value;
    }
}

public record ExcluiOrcamentoCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}