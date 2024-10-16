using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Usuario;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Solados.RetornaSolados;

public class RetornaSoladosHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaSoladosQuery, IList<SoladoModel>>
{
    public async Task<IList<SoladoModel>> Handle(RetornaSoladosQuery request, CancellationToken cancellationToken)
    {
        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = request.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery);

        if (usuario.ExibirSoladoPalmilhaSidiWeb != "S")
        {
            return [];
        }

        var sql = new StringBuilder("SELECT CODIGO_INTERNO Codigo, ITEM_ESTOQUE.DESCRICAO Descricao,");
        sql.AppendSql("ITEM_ESTOQUE.DESCRICAO || ' - ' || CORES.DESCRICAO DescricaoCor, CORES.DESCRICAO Cor");
        sql.AppendSql("FROM ITEM_ESTOQUE");
        sql.AppendSql("LEFT JOIN CORES ON(ITEM_ESTOQUE.CODIGO_COR = CORES.CODIGO)");
        sql.AppendSql("WHERE FK_TIPO_ITEM_ESTOQUE = 2");
        sql.AppendSql("ORDER BY ITEM_ESTOQUE.DESCRICAO, CORES.DESCRICAO");

        var solados = await conexao.QueryAsync<SoladoModel>(sql.ToString(), cancellationToken);
        return solados.ToList();
    }
}

public record RetornaSoladosQuery : IRequest<IList<SoladoModel>> {
    public required int UsuarioCodigo { get; init; }
}
