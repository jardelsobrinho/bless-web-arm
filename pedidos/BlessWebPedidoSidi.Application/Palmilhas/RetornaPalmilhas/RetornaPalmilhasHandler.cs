using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Usuario;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Palmilhas.RetornaPalmilhas;

public class RetornaPamilhasHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaPalmilhasQuery, IList<PalmilhaModel>>
{
    public async Task<IList<PalmilhaModel>> Handle(RetornaPalmilhasQuery request, CancellationToken cancellationToken)
    {
        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = request.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery, cancellationToken);

        if (usuario.ExibirSoladoPalmilhaSidiWeb != "S")
        {
            return [];
        }

        var sql = new StringBuilder("SELECT CODIGO_INTERNO Codigo, ITEM_ESTOQUE.DESCRICAO Descricao,");
        sql.AppendSql("ITEM_ESTOQUE.DESCRICAO || ' - ' || CORES.DESCRICAO DescricaoCor, CORES.DESCRICAO Cor");
        sql.AppendSql("FROM ITEM_ESTOQUE");
        sql.AppendSql("LEFT JOIN CORES ON(ITEM_ESTOQUE.CODIGO_COR = CORES.CODIGO)");
        sql.AppendSql("WHERE FK_TIPO_ITEM_ESTOQUE = 3");
        sql.AppendSql("ORDER BY ITEM_ESTOQUE.DESCRICAO, CORES.DESCRICAO");

        var palmilhas = await conexao.QueryAsync<PalmilhaModel>(sql.ToString(), cancellationToken);
        return palmilhas.ToList();
    }
}

public record RetornaPalmilhasQuery : IRequest<IList<PalmilhaModel>>
{
    public required int UsuarioCodigo { get; init; }
}
