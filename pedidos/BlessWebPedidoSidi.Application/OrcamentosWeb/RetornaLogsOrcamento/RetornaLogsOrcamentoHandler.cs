using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaLogsOrcamento;

public class RetornaLogsOrcamentoHandler(IDbConnection conexao) : IRequestHandler<RetornaLogsOrcamentoQuery, IList<RetornaLogsOrcamentoModel>>
{
    public async Task<IList<RetornaLogsOrcamentoModel>> Handle(RetornaLogsOrcamentoQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("select wol.Status, wol.Descricao, wol.Data from web_orcamento wo");
        sql.AppendSql("inner join web_orcamento_log wol on wol.ORCAMENTO_ID = wo.ID");
        sql.AppendSql("where wo.UUID = @UUID");
        sql.AppendSql("order by wol.Data DESC");

        var param = new { UUID = query.Uuid };

        var consulta = await conexao.QueryAsync<RetornaLogsOrcamentoModel>(sql.ToString(), param);
        return consulta.ToList();
    }
}

public record RetornaLogsOrcamentoQuery : IRequest<IList<RetornaLogsOrcamentoModel>>
{
    public required string Uuid { get; set; }
}
