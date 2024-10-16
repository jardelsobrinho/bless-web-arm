using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RemoveItemOrcamento;

public class RemoveItemOrcamentoHandler(IMediator mediator) : IRequestHandler<RemoveItemOrcamentoCommand, Unit>
{
    public async Task<Unit> Handle(RemoveItemOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        if (orcamento.Itens.Where(x => x.Id == command.OrcamentoItemId).ToList().Count == 0)
            throw new BadHttpRequestException("RIOH01 - Item não encontrado no orçamento");

        orcamento.Itens = orcamento.Itens.Where(x => x.Id != command.OrcamentoItemId).ToList();

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);
        return Unit.Value;
    }
}

public record RemoveItemOrcamentoCommand : IRequest<Unit>
{
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required string Uuid { get; set; }
    public required int OrcamentoItemId { get; set; }
}