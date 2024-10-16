using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using static iText.IO.Image.Jpeg2000ImageData;

namespace BlessWebPedidoSidi.Application.Cidades;

public class RetornaCodigoCidadeHandler(IDbConnection conexao) : IRequestHandler<RetornaCodigoCidadeQuery, int>
{
    public async Task<int> Handle(RetornaCodigoCidadeQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("SELECT C.CODIGO Codigo FROM CIDADE C WHERE 1=1");

        var sqlPesquisa = new StringBuilder(sql.ToString());
        var filtros = new Dictionary<string, object>();

        if (!query.UfSigla.IsNullOrEmpty())
        {
            sqlPesquisa.AppendSql("AND UF = @UF_CIDADE");
            filtros.Add("UF_CIDADE", query.UfSigla.ToUpper());
        }

        var sqlPorNomeCidade = new StringBuilder(sqlPesquisa.ToString());
        if (!query.CidadeNome.IsNullOrEmpty())
        {
            sqlPorNomeCidade.AppendSql("AND nome like @CIDADE_NOME");
            filtros.Add("CIDADE_NOME", query.CidadeNome.ToUpper() + "%");
        }

        var parameters = new DynamicParameters(filtros);
        var cidadeCodigo = (await conexao.QueryAsync<int>(sqlPorNomeCidade.ToString(), parameters)).FirstOrDefault();

        if (cidadeCodigo > 0)
        {
            return cidadeCodigo;
        }

        filtros.Clear();
        var sqlPorCep = new StringBuilder(sql.ToString());
        if (!query.CidadeCep.IsNullOrEmpty())
        {
            sqlPorCep.AppendSql("AND cep = @CIDADE_CEP");
            filtros.Add("CIDADE_CEP", Regex.Match(query.CidadeCep, @"\d+").Value);
        }
        var parametros = new DynamicParameters(filtros);

        return (await conexao.QueryAsync<int>(sqlPorCep.ToString(), parametros)).FirstOrDefault();
    }
}

public record RetornaCodigoCidadeQuery : IRequest<int>
{
    public required string UfSigla { get; set; }
    public required string CidadeCep { get; set; }
    public required string CidadeNome { get; set; }
}
