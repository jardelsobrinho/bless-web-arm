using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.NovoOrcamento;

public class NovoOrcamentoHandler(IUnitOfWork unitOfWork) : IRequestHandler<NovoOrcamentoCommand, string>
{
    public async Task<string> Handle(NovoOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var uuid = Guid.NewGuid().ToString();

        var orcamentoEntity = new OrcamentoWebEntity()
        {
            DataCriacao = DateTime.Now,
            EmpresaCnpj = command.EmpresaCnpj,
            Id = 0,
            RepresentanteCnpj = command.RepresentanteCnpj,
            UsuarioCodigo = command.UsuarioCodigo,
            ValorTotal = 0,
            Uuid = uuid,
            Status = EOrcamentoStatus.Aberto
        };

        await unitOfWork.StartTransactionAsync();

        try
        {
            await unitOfWork.OrcamentoWebRepository.GravaAsync(orcamentoEntity, 0);
            await unitOfWork.CommitTransactionAsync();
            return uuid;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new BadHttpRequestException("NOH01 - Erro ao gravar o orcamento", ex);
        }
    }
}

public record NovoOrcamentoCommand : IRequest<string>
{
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}