using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Cidades;

public class RetornaCidadesHandler(IDbConnection conexao) : IRequestHandler<RetornaCidadesQuery, IList<CidadeModel>>
{
    public async Task<IList<CidadeModel>> Handle(RetornaCidadesQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("SELECT C.CODIGO Codigo, C.NOME, C.CODIGO_IBGE CodigoIbge, Nome FROM CIDADE C WHERE 1=1");

        var filtros = new Dictionary<string, object>();

        if (!query.UfSigla.IsNullOrEmpty())
        {
            sql.AppendSql("AND UF = @UF_CIDADE");
            filtros.Add("UF_CIDADE", query.UfSigla.ToUpper());
        }

        if (!query.CidadeNome.IsNullOrEmpty())
        {
            sql.AppendSql("AND nome like @CIDADE_NOME");
            filtros.Add("CIDADE_NOME", query.CidadeNome.ToUpper() + "%");
        }

        sql.AppendSql("ORDER BY C.NOME");

        var parameters = new DynamicParameters(filtros);
        var listaCidades = await conexao.QueryAsync<CidadeModel>(sql.ToString(), parameters);
        return listaCidades.ToList();
    }
}

public record RetornaCidadesQuery : IRequest<IList<CidadeModel>>
{
    public required string UfSigla { get; init; }
    public required string CidadeNome { get; init; }
}