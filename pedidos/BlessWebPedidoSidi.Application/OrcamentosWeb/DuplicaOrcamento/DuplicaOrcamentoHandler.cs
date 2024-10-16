using BlessWebPedidoSidi.Domain.Shared;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.DuplicaOrcamento;

public class DuplicaOrcamentoHandler(IUnitOfWork unitOfWork) : IRequestHandler<DuplicaOrcamentoCommand, DuplicaOrcamentoModel>
{
    public async Task<DuplicaOrcamentoModel> Handle(DuplicaOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var novoUuid = Guid.NewGuid().ToString();

        try
        {
            await unitOfWork.StartTransactionAsync();
            var orcamentoEntity = await unitOfWork.OrcamentoWebRepository.DuplicaAsync(novoUuid, command.Uuid, command.RepresentanteCnpj, command.UsuarioCodigo);
            await unitOfWork.CommitTransactionAsync();
            return new DuplicaOrcamentoModel(novoUuid, orcamentoEntity.Itens.Count > 0);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new Exception("DOH01 - Falha ao duplicar o orçamento", ex);
        }
    }
}

public record DuplicaOrcamentoCommand : IRequest<DuplicaOrcamentoModel>
{
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required string Uuid { get; set; }
}

public record DuplicaOrcamentoModel(string Uuid, bool PossuiProdutos);

