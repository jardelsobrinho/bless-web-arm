using BlessWebPedidoSidi.Application.Usuario;
using Dapper;
using MediatR;
using System.Data;

namespace BlessWebPedidoSidi.Application.Marcas;

public class RetornaMarcaHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaMarcaQuery, IList<MarcaModel>>
{
    public async Task<IList<MarcaModel>> Handle(RetornaMarcaQuery query, CancellationToken cancellationToken)
    {
        var usuario = await mediator.Send(new UsuarioQuery() {UsuarioCodigo = query.UsuarioCodigo}, cancellationToken);

        if (usuario.ExibirMarcaVendasWeb == "S") { 
            var sql = "SELECT M.CODIGO, M.NOME_MARCA NOME FROM MARCA M WHERE (M.INATIVA <> 'T') OR (M.INATIVA IS NULL) ORDER BY M.NOME_MARCA";
            var listaMarcas = await conexao.QueryAsync<MarcaModel>(sql);
            return listaMarcas.ToList();
        }

        return [];
    }
}

public record RetornaMarcaQuery : IRequest<IList<MarcaModel>> {
    public required int UsuarioCodigo { get; init; }
}
