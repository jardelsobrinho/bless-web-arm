using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaPrecos;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.Shared;
using Dapper;
using MediatR;
using System.Data;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;

public class GravaOrcamentoHandler(IMediator mediator, IUnitOfWork unitOfWork, IDbConnection conexao) : IRequestHandler<GravaOrcamentoCommand, OrcamentoWebEntity>
{
    public async Task<OrcamentoWebEntity> Handle(GravaOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var orcamento = command.OrcamentoWeb;

        if (command.ValidaDados)
            await ValidaModeloAtivo(orcamento, cancellationToken);

        var atualizaPrecosCommand = new AtualizaPrecosCommand() { OrcamentoWeb = orcamento };
        orcamento = await mediator.Send(atualizaPrecosCommand, cancellationToken);

        if (command.ValidaDados)
            orcamento.ValidaDados();

        try
        {
            await unitOfWork.StartTransactionAsync();
            await unitOfWork.OrcamentoWebRepository.GravaAsync(orcamento, orcamento.Id);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new Exception("GOH01 - Falha ao gravar o orçamento", ex);
        }
        return orcamento;
    }

    private async Task ValidaModeloAtivo(OrcamentoWebEntity orcamento, CancellationToken cancellationToken)
    {
        foreach (var item in orcamento.Itens)
        {
            var sqlModeloAtivo = $"SELECT COUNT(*) FROM MODELOS M WHERE M.MODELO = {item.ModeloCodigo} AND M.BLOQ_PED_SIDI = 'S'";
            var countModeloAtivo = (await conexao.QueryAsync<int>(sqlModeloAtivo, cancellationToken)).First(); ;
            if (countModeloAtivo == 0)
            {
                throw new Exception($"GOH02 - Modelo {item.ModeloCodigo}-{item.ModeloDescricao} está bloqueado para venda online");
            }

            var sqlModeloCorAtivo = $"SELECT COUNT(*) FROM FICHA_TECNICA_HD F WHERE F.FK_MODELO = {item.ModeloCodigo} AND F.VERSAO = {item.CorCodigo} AND F.BLOQ_PED_SIDI = 'S'";
            var countModeloCorAtivo = (await conexao.QueryAsync<int>(sqlModeloCorAtivo, cancellationToken)).First(); ;
            if (countModeloCorAtivo == 0)
            {
                throw new Exception($"GOH03 - Modelo {item.ModeloCodigo}-{item.ModeloDescricao} na cor {item.CorCodigo} - {item.CorDescricao} está bloqueado para venda online");
            }
        }
    }
}

public record GravaOrcamentoCommand : IRequest<OrcamentoWebEntity>
{
    public required OrcamentoWebEntity OrcamentoWeb { get; init; }
    public bool ValidaDados { get; init; } = false;
}