using AutoMapper;
using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.EstoqueAcabado;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Shared.GeraCaminhoImagem;
using BlessWebPedidoSidi.Application.Tamanhos;
using BlessWebPedidoSidi.Application.Usuario;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento;

public class RetornaOrcamentoHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator,
    IDbConnection conexao, IMarcaRepository marcaRepository) : IRequestHandler<RetornaOrcamentoQuery, OrcamentoModel>
{
    public async Task<OrcamentoModel> Handle(RetornaOrcamentoQuery command, CancellationToken cancellationToken)
    {
        var orcamento = await unitOfWork.OrcamentoWebRepository.CarregarDadosAsync(
        x => x.Uuid == command.Uuid && x.RepresentanteCnpj == command.RepresentanteCnpj
        && x.UsuarioCodigo == command.UsuarioCodigo && x.Status == command.Status)
            ?? throw new BadHttpRequestException($"RHO01 - Orçamento não encontrado para o uuid {command.Uuid}");
        var orcamentoModel = mapper.Map<OrcamentoModel>(orcamento);

        var usuario = await mediator.Send(new UsuarioQuery() { UsuarioCodigo = command.UsuarioCodigo }, cancellationToken);
        orcamentoModel.ExibirObservacaoItem = usuario.ExibirObsItemSidiWeb == "S";

        var controleSistema = await mediator.Send(new ControleSistemaQuery(), cancellationToken);
        var controleSistemaPedido = await mediator.Send(new ControleSistemaPedidoQuery(), cancellationToken);

        if (orcamentoModel.TabelaPrecoCodigo != null)
        {
            var sqlTabelaPreco = $"SELECT T.NOME FROM TABELA_PRECOS_HD T WHERE T.CODIGO = {orcamentoModel.TabelaPrecoCodigo}";
            var tabelaPrecoNome = (await conexao.QueryAsync<string>(sqlTabelaPreco)).FirstOrDefault();
            orcamentoModel.TabelaPrecoNome = tabelaPrecoNome;
        }

        if (orcamentoModel.CondicaoPagamentoCodigo != null)
        {
            var sqlCondicaoPagamento = $"SELECT C.DESCRICAO FROM CONDICAO_PAGAMENTO C WHERE C.CODIGO = {orcamentoModel.CondicaoPagamentoCodigo}";
            var condicaoPagamentoDescricao = (await conexao.QueryAsync<string>(sqlCondicaoPagamento)).FirstOrDefault();
            orcamentoModel.CondicaoPagamentoDescricao = condicaoPagamentoDescricao;

        }

        if (command.ModeloCodigo != 0 && command.CorCodigo != 0)
        {
            orcamentoModel.Itens = orcamentoModel.Itens.Where(x => x.ModeloCodigo == command.ModeloCodigo && x.CorCodigo == command.CorCodigo).ToList();
        }

        orcamentoModel.ValidaEstoque = controleSistemaPedido.ValidaEstoqueDisponivel == "T" ||
                controleSistemaPedido.ValidaEstoqueAcabadoSibMobile == "T";

        foreach (var item in orcamentoModel.Itens)
        {
            var retornaTamanhosQuery = new RetornaTamanhosQuery()
            {
                TipoCodigo = TipoCodigoRetornaTamanho.Modelo,
                Codigo = item.ModeloCodigo
            };

            var listaTamanhos = await mediator.Send(retornaTamanhosQuery, cancellationToken);

            if (item.MarcaCodigo != null)
            {
                item.MarcaDescricao = await marcaRepository.RetornaNomeMarca(item.MarcaCodigo ?? 0);
            }

            if (item.PalmilhaCodigo != null)
            {
                var sqlDescricaoPalmilha = new StringBuilder("SELECT ITEM_ESTOQUE.DESCRICAO || ' - ' || CORES.DESCRICAO DescricaoCor");
                sqlDescricaoPalmilha.AppendSql("FROM ITEM_ESTOQUE");
                sqlDescricaoPalmilha.AppendSql("LEFT JOIN CORES ON(ITEM_ESTOQUE.CODIGO_COR = CORES.CODIGO)");
                sqlDescricaoPalmilha.AppendSql($"WHERE FK_TIPO_ITEM_ESTOQUE = 3 AND ITEM_ESTOQUE.CODIGO_INTERNO = {item.PalmilhaCodigo}");

                item.PalmilhaDescricao = (await conexao.QueryAsync<string>(sqlDescricaoPalmilha.ToString())).FirstOrDefault();
            }

            if (item.SoladoCodigo != null)
            {
                var sqlDescricaoSolado = new StringBuilder("SELECT ITEM_ESTOQUE.DESCRICAO || ' - ' || CORES.DESCRICAO DescricaoCor");
                sqlDescricaoSolado.AppendSql("FROM ITEM_ESTOQUE");
                sqlDescricaoSolado.AppendSql("LEFT JOIN CORES ON(ITEM_ESTOQUE.CODIGO_COR = CORES.CODIGO)");
                sqlDescricaoSolado.AppendSql($"WHERE FK_TIPO_ITEM_ESTOQUE = 2 AND ITEM_ESTOQUE.CODIGO_INTERNO = {item.SoladoCodigo}");

                item.SoladoDescricao = (await conexao.QueryAsync<string>(sqlDescricaoSolado.ToString())).FirstOrDefault();
            }

            var geraCaminhoImagem = new GeraCaminhoImagemCommand()
            {
                CorCodigo = item.CorCodigo,
                ModeloCodigo = item.ModeloCodigo
            };
            item.Imagem = await mediator.Send(geraCaminhoImagem, cancellationToken);

            var sqlReferencia = $"SELECT L.DESCRICAO ReferenciaModelo FROM MODELOS M LEFT JOIN LINHA L ON (L.CODIGO = M.FK_LINHA) WHERE M.MODELO = {item.ModeloCodigo}";
            var codReferencia = (await conexao.QueryAsync<string>(sqlReferencia)).FirstOrDefault();
            item.ReferenciaModelo = codReferencia ?? "";

            var estoqueAcabadoQuery = new EstoqueAcabadoQuery()
            {
                CorCodigo = item.CorCodigo,
                ModeloCodigo = item.ModeloCodigo,
                UsuarioCodigo = command.UsuarioCodigo
            };

            foreach (var tamanho in item.Grade)
            {

                if (controleSistemaPedido.ValidaEstoqueAcabadoSibMobile == "T")
                {
                    var listaEstoques = await mediator.Send(estoqueAcabadoQuery, cancellationToken);

                    tamanho.Estoque = (from e in listaEstoques
                                       where e.TamanhoCodigo == tamanho.TamanhoCodigo
                                       select e.Estoque).SingleOrDefault();
                }

                var tamanhoDescricaoOrdem = (from t in listaTamanhos
                                             where t.Codigo == tamanho.TamanhoCodigo
                                             select t).SingleOrDefault();

                if (tamanhoDescricaoOrdem != null)
                {
                    tamanho.TamanhoDescricao = tamanhoDescricaoOrdem.Descricao;
                    tamanho.Ordem = tamanhoDescricaoOrdem.Ordem;
                }
            }

            item.Grade = [.. item.Grade.OrderBy(x => x.Ordem)
                                        .ThenBy(x => x.TamanhoDescricao)];
        }
        return orcamentoModel;
    }

}

public record RetornaOrcamentoQuery : IRequest<OrcamentoModel>
{
    public required string Uuid { get; init; }
    public required string RepresentanteCnpj { get; init; }
    public required int UsuarioCodigo { get; init; }
    public int ModeloCodigo { get; init; }
    public int CorCodigo { get; init; }
    public required EOrcamentoStatus Status { get; init; }
}
