using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;

public class RetornaOrcamentoParaEdicaoHandler(IUnitOfWork unitOfWork) : IRequestHandler<RetornaOrcamentoParaEdicaoQuery, OrcamentoWebEntity>
{
    public async Task<OrcamentoWebEntity> Handle(RetornaOrcamentoParaEdicaoQuery query, CancellationToken cancellationToken)
    {
        var orcamento = await unitOfWork.OrcamentoWebRepository.CarregarDadosAsync(
        x => x.Uuid == query.Uuid && x.RepresentanteCnpj == query.RepresentanteCnpj
        && x.UsuarioCodigo == query.UsuarioCodigo)
            ?? throw new BadHttpRequestException($"ROPEH01 - Orçamento não encontrado para o uuid {query.Uuid}");

        if (orcamento.Status == EOrcamentoStatus.Finalizado)
            throw new BadHttpRequestException($"ROPEH02 - Esse orçamento já está finalizado");

        if (orcamento.Status != EOrcamentoStatus.Aberto)
            throw new BadHttpRequestException($"ROPEH03 - Esse orçamento não está Aberto ele está como status {orcamento.Status}. ");

        return orcamento;
    }
}

public record RetornaOrcamentoParaEdicaoQuery : IRequest<OrcamentoWebEntity>
{
    public required string Uuid { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}