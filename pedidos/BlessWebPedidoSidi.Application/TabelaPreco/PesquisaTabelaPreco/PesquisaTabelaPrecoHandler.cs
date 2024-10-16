using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.TabelaPreco;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.TabelaPreco.PesquisaTabelaPreco;

public class PesquisaTabelaPrecoHandler(IMediator mediator, IDbConnection conexao) : IRequestHandler<PesquisaTabelaPrecoQuery, IList<TabelaPrecoModel>>
{
    public async Task<IList<TabelaPrecoModel>> Handle(PesquisaTabelaPrecoQuery query, CancellationToken cancellationToken)
    {
        var controleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(controleSistemaPedidoQuery, cancellationToken);

        var sql = new StringBuilder();
        var filtros = new Dictionary<string, object>();

        if (controleSistemaPedido.SelecionaTabelaPreco == "S")
        {
            sql.AppendSql("SELECT X.CODIGO Codigo, X.NOME Nome, X.TABELA_PADRAO TabelaPadrao");

            sql.AppendSql("FROM(");
            sql.AppendSql("SELECT TPHD.CODIGO, TPHD.NOME, 'S' AS TABELA_PADRAO");
            sql.AppendSql("FROM TABELA_PRECOS_HD TPHD");
            sql.AppendSql("INNER JOIN FORNECEDOR F ON (TPHD.CODIGO = F.TABELA_PRECO)");
            sql.AppendSql("WHERE F.CGC = @REPRESENTANTE_CNPJ");

            if (query.TabelaPrecoNome != "")
                sql.AppendSql(" AND TPHD.NOME LIKE @NOME");

            if (query.TabelaPrecoCodigo > 0)
                sql.AppendSql(" AND TPHD.CODIGO = @TABELA_PRECO_CODIGO");

            sql.AppendSql("UNION");
            sql.AppendSql("SELECT TPHD.CODIGO, TPHD.NOME, 'N' AS TABELA_PADRAO");
            sql.AppendSql("FROM TABELA_PRECOS_HD TPHD");
            sql.AppendSql("INNER JOIN TABELA_PRECOS_REPRESENTANTE TPR ON (TPHD.CODIGO = TPR.FK_TABELA_HD)");
            sql.AppendSql("WHERE TPR.CGC_REPRESENTANTE = @REPRESENTANTE_CNPJ");

            if (query.TabelaPrecoNome != "")
                sql.AppendSql(" AND TPHD.NOME LIKE @NOME");

            if (query.TabelaPrecoCodigo > 0)
                sql.AppendSql(" AND TPHD.CODIGO = @TABELA_PRECO_CODIGO");

            sql.AppendSql("AND (SELECT COUNT(*) FROM FORNECEDOR F");
            sql.AppendSql("WHERE F.CGC = @REPRESENTANTE_CNPJ AND F.TABELA_PRECO = TPHD.CODIGO) = 0) X");

            sql.AppendSql("ORDER BY X.Nome");
        }
        else
        {
            sql.AppendSql("SELECT TPHD.CODIGO Codigo, TPHD.NOME Nome, BLOQ_SIDI_MOBILE ");

            sql.AppendSql("FROM TABELA_PRECOS_HD TPHD ");
            sql.AppendSql("INNER JOIN FORNECEDOR F ON(TPHD.CODIGO = F.TABELA_PRECO) ");
            sql.AppendSql("WHERE F.CGC = @REPRESENTANTE_CNPJ ");

            if (query.TabelaPrecoNome != "")
                sql.AppendSql(" AND TPHD.NOME LIKE @NOME");

            if (query.TabelaPrecoCodigo > 0)
                sql.AppendSql(" AND TPHD.CODIGO = @TABELA_PRECO_CODIGO");

            sql.AppendSql("ORDER BY Nome");
        }

        if (query.TabelaPrecoNome != "")
            filtros.Add("@NOME", query.TabelaPrecoNome + "%");

        if (query.TabelaPrecoCodigo > 0)
            filtros.Add("@TABELA_PRECO_CODIGO", query.TabelaPrecoCodigo);

        filtros.Add("@REPRESENTANTE_CNPJ", query.RepresentanteCnpj);
        var parameters = new DynamicParameters(filtros);

        var registros = (await conexao.QueryAsync<TabelaPrecoModel>(sql.ToString(), parameters)).ToList();
        return registros;
    }
}

public record PesquisaTabelaPrecoQuery : IRequest<IList<TabelaPrecoModel>>
{
    public required string TabelaPrecoNome { get; set; }
    public required int TabelaPrecoCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
}