using Dapper;
using MediatR;
using System.Data;

namespace BlessWebPedidoSidi.Application.Ufs;

public class RetornaUfHandler(IDbConnection conexao) : IRequestHandler<RetornaUfsQuery, IList<UfModel>>
{
    public async Task<IList<UfModel>> Handle(RetornaUfsQuery query, CancellationToken cancellationToken)
    {
        var sql = "SELECT UF.SIGLA_UF Sigla, UF.DESCRICAO Descricao FROM UF ORDER BY UF.DESCRICAO";
        var listaUfs = await conexao.QueryAsync<UfModel>(sql);
        return listaUfs.ToList();
    }
}

public class RetornaUfsQuery : IRequest<IList<UfModel>>;