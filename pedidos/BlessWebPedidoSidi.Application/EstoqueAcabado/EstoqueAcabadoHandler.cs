using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.Usuario;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.EstoqueAcabado;

public class EstoqueAcabadoHandler(IMediator mediator, IDbConnection conexao) : IRequestHandler<EstoqueAcabadoQuery, IList<EstoqueAcabadoModel>>
{

    public async Task<IList<EstoqueAcabadoModel>> Handle(EstoqueAcabadoQuery query, CancellationToken cancellationToken)
    {
        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = query.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery, cancellationToken);

        var controleSistemaQuery = new ControleSistemaQuery();
        var controleSistema = await mediator.Send(controleSistemaQuery, cancellationToken);

        var pesquisaControleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(pesquisaControleSistemaPedidoQuery, cancellationToken);

        var fabricaCodigo = usuario.Fabrica;
        if (fabricaCodigo == 0)
            fabricaCodigo = controleSistema.FabricaPadrao;

        var sqlFabrica = $"SELECT F.FABRICA_ESTOQUE FROM FABRICA F WHERE F.CODIGO = {fabricaCodigo}";
        var fabricaEstoqueCodigo = (await conexao.QueryAsync<int?>(sqlFabrica)).SingleOrDefault() ?? 0;
        if (fabricaEstoqueCodigo != 0)
            fabricaCodigo = fabricaEstoqueCodigo;

        var filtros = new Dictionary<string, object>
        {
            { "@MODELO_CODIGO", query.ModeloCodigo },
            { "@COR_CODIGO", query.CorCodigo },
        };

        var sql = new StringBuilder();
        if (controleSistemaPedido.ValidaEstoqueDisponivel == "T")
        {
            sql.AppendSql("SELECT E.MODELO ModeloCodigo, E.VERSAO CorCodigo, E.TAMANHO_CALCADO TamanhoCodigo, E.QUANTIDADE Estoque");
            sql.AppendSql("FROM ESTOQUE_DISPONIVEL E WHERE E.MODELO = @MODELO_CODIGO AND E.VERSAO = @COR_CODIGO");
        }
        else
        {
            sql.AppendSql("SELECT E.MODELO ModeloCodigo, E.VERSAO CorCodigo,");
            sql.AppendSql("E.TAMANHO_CALCADO TamanhoCodigo, E.QTE Estoque FROM ESTOQUE_ACABADO E");
            sql.AppendSql("WHERE E.MODELO = @MODELO_CODIGO AND E.VERSAO = @COR_CODIGO");
            sql.AppendSql("AND E.FABRICA = @FABRICA AND E.TIPO_ESTOQUE = @TIPO_ESTOQUE");

            filtros.Add("@FABRICA", fabricaCodigo);
            filtros.Add("@TIPO_ESTOQUE", 1);
        }

        var parameters = new DynamicParameters(filtros);

        var listaEstoques = (await conexao.QueryAsync<EstoqueAcabadoModel>(sql.ToString(), parameters)).ToList();
        return listaEstoques;
    }
}

public record EstoqueAcabadoQuery : IRequest<IList<EstoqueAcabadoModel>>
{
    public required int ModeloCodigo { get; set; }
    public required int CorCodigo { get; set; }
    public required int UsuarioCodigo { get; set; }
}