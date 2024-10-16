using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.EstoqueAcabado;
using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.FinalizaOrcamento;

public class FinalizaOrcamentoHandler(IMediator mediator) : IRequestHandler<FinalizaOrcamentoCommand, Unit>
{
    public async Task<Unit> Handle(FinalizaOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        orcamento.Frete = command.Frete;
        orcamento.TipoEstoqueCodigo = "C";
        orcamento.Status = EOrcamentoStatus.Finalizado;
        orcamento.DataEmissao = DateTime.Now;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand()
        {
            OrcamentoWeb = orcamento,
            ValidaDados = true
        };

        //await ValidaEstoqueOrcamento(orcamento, cancellationToken);
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }

    private async Task ValidaEstoqueOrcamento(OrcamentoWebEntity orcamento, CancellationToken cancellationToken)
    {
        var pesquisaControleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(pesquisaControleSistemaPedidoQuery, cancellationToken);

        foreach (var item in orcamento.Itens)
        {
            var estoqueAcabadoQuery = new EstoqueAcabadoQuery()
            {
                CorCodigo = item.CorCodigo,
                ModeloCodigo = item.ModeloCodigo,
                UsuarioCodigo = orcamento.UsuarioCodigo

            };
            var listaEstoques = await mediator.Send(estoqueAcabadoQuery, cancellationToken);

            foreach (var tamanho in item.Grade)
            {
                var tamanhoEstoque = listaEstoques.Where(x => x.TamanhoCodigo == tamanho.TamanhoCodigo)
                    .SingleOrDefault();

                if (controleSistemaPedido.ValidaEstoqueDisponivel == "T"
                    || controleSistemaPedido.ValidaEstoqueAcabadoSibMobile == "T")
                {
                    if (tamanhoEstoque == null)
                    {
                        throw new BadHttpRequestException("FOH01 - Não existe estoque disponível para o modelo" +
                        $" {item.ModeloCodigo}-{item.ModeloDescricao} na cor {item.CorCodigo}-{item.CorDescricao}" +
                        $" para o tamanho {tamanho.TamanhoDescricao}.");
                    }

                    if (tamanhoEstoque.Estoque < tamanho.Quantidade)
                    {
                        _ = new BadHttpRequestException("fho02 - Não existe estoque suficiente disponível para o modelo" +
                                            $" {item.ModeloCodigo}-{item.ModeloDescricao} na cor {item.CorCodigo}-{item.CorDescricao}" +
                                            $" para o tamanho {tamanho.TamanhoDescricao}. Estoque disponivel {tamanhoEstoque.Estoque}," +
                                            $" quantidade necessária {tamanho.Quantidade}");
                    }
                }
            }
        }
    }
}

public record FinalizaOrcamentoCommand : IRequest<Unit>
{
    public required string Frete { get; init; }
    public required string Uuid { get; init; }
    public required string RepresentanteCnpj { get; init; }
    public required int UsuarioCodigo { get; init; }
}