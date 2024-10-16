using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.TabelaPreco.ValidaTabelaRepresentante;
using BlessWebPedidoSidi.Application.Usuario;
using BlessWebPedidoSidi.Application.CondicaoPagamento;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.CondicaoPagamento.PesquisaCondicaoPagamento;

public class PesquisaCondicaoPagamentoHandler(IMediator mediator, IDbConnection conexao) : IRequestHandler<PesquisaCondicaoPagamentoQuery, IList<CondicaoPagamentoModel>>
{

    public async Task<IList<CondicaoPagamentoModel>> Handle(PesquisaCondicaoPagamentoQuery query, CancellationToken cancellationToken)
    {
        var queryValidaTabela = new ValidaTabelaRepresentanteQuery()
        {
            RepresentanteCnpj = query.RepresentanteCnpj,
            TabelaPrecoCodigo = query.TabelaPrecoCodigo
        };

        var tabelaCodigoValido = await mediator.Send(queryValidaTabela, cancellationToken);
        if (!tabelaCodigoValido)
            throw new BadHttpRequestException("PCPH01 - Codigo da tabela de preço inválido para esse representante");

        var sql = new StringBuilder();
        var filtros = new Dictionary<string, object>();

        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = query.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery, cancellationToken);

        if (usuario.ExibirTodasCondPagtos == "S")
        {
            sql.AppendSql("SELECT DISTINCT X.CODIGO Codigo, X.DESCRICAO Descricao, X.PRAZO_MEDIO PrazoMedio");
            sql.AppendSql("FROM(SELECT CODIGO, DESCRICAO, PRAZO_MEDIO");
            sql.AppendSql("FROM CONDICAO_PAGAMENTO");
            sql.AppendSql("WHERE CODIGO = PRAZO_MEDIO");
            sql.AppendSql("UNION ALL");
            sql.AppendSql("SELECT CODIGO, DESCRICAO, PRAZO_MEDIO");
            sql.AppendSql("FROM CONDICAO_PAGAMENTO");
            sql.AppendSql("WHERE ATIVO = 'S' AND PEDIDO_SIDI = 'S'");
            sql.AppendSql(") X");
            sql.AppendSql("ORDER BY X.DESCRICAO");
        }
        else
        {
            sql.AppendSql("SELECT DISTINCT CODIGO Codigo, DESCRICAO Descricao, PRAZO_MEDIO PrazoMedio");
            sql.AppendSql("FROM TABELA_PRECOS TP");
            sql.AppendSql("INNER JOIN TABELA_PRECOS_CONDICOES TPC ON TPC.CODIGO_TABELA = TP.CODIGO_TABELA");
            sql.AppendSql("INNER JOIN CONDICAO_PAGAMENTO CP ON CP.CODIGO = TPC.CODIGO_PAGTO");
            sql.AppendSql("WHERE CP.PEDIDO_SIDI = 'S' ");
            sql.AppendSql("AND CP.ATIVO = 'S'");

            sql.AppendSql("AND TP.CODIGO_TABELA_HD = @TABELA_PRECO_CODIGO");
            filtros.Add("@TABELA_PRECO_CODIGO", query.TabelaPrecoCodigo.ToString());

            sql.AppendSql("GROUP BY CODIGO, DESCRICAO, PRAZO_MEDIO");
            sql.AppendSql("ORDER BY DESCRICAO");
        }

        var parameters = new DynamicParameters(filtros);
        var listaCondPagtos = (await conexao.QueryAsync<CondicaoPagamentoModel>(sql.ToString(), parameters)).ToList();

        var queryControleSistemaPedido = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(queryControleSistemaPedido, cancellationToken);
        var condicoesPagamentoGeral = listaCondPagtos.ToList();
        if (controleSistemaPedido.FiltrarPrazoMedioCondicoes == "S")
        {
            foreach (var condicao in listaCondPagtos)
            {
                if (condicao.Codigo != condicao.PrazoMedio)
                    continue;

                var sqlConsulta = new StringBuilder();
                sqlConsulta.AppendSql("SELECT CODIGO, DESCRICAO, PRAZO_MEDIO PrazoMedio");
                sqlConsulta.AppendSql("FROM CONDICAO_PAGAMENTO");
                sqlConsulta.AppendSql("WHERE ATIVO = 'S' AND PEDIDO_SIDI = 'S'");
                sqlConsulta.AppendSql("AND PRAZO_MEDIO = @PRAZO_MEDIO AND CODIGO <> @PRAZO_MEDIO");

                filtros.Clear();
                filtros.Add("@PRAZO_MEDIO", condicao.Codigo);

                var parametrosCondicao = new DynamicParameters(filtros);
                var listaCondPagtosPrazoMedio = (await conexao.QueryAsync<CondicaoPagamentoModel>(sqlConsulta.ToString(), parametrosCondicao)).ToList();
                condicoesPagamentoGeral.AddRange(listaCondPagtosPrazoMedio);
            }

        }

        var condicoesPagamentoOrdenada = condicoesPagamentoGeral
                .Where(x => x.Descricao.ToUpper().Contains(query.CondicaoPagamentoDescricao.ToUpper()) || query.CondicaoPagamentoDescricao == "")
                .Distinct()
                .OrderBy(x => x.Descricao).ToList();

        return condicoesPagamentoOrdenada;
    }
}

public record PesquisaCondicaoPagamentoQuery : IRequest<IList<CondicaoPagamentoModel>>
{
    public required string CondicaoPagamentoDescricao { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int TabelaPrecoCodigo { get; set; }
    public required int UsuarioCodigo { get; set; }
}