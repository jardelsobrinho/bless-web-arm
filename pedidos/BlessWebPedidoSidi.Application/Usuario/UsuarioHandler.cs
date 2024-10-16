using Dapper;
using MediatR;
using System.Data;

namespace BlessWebPedidoSidi.Application.Usuario;

public class UsuarioHandler(IDbConnection conexao) : IRequestHandler<UsuarioQuery, UsuarioModel>
{
    public async Task<UsuarioModel> Handle(UsuarioQuery query, CancellationToken cancellationToken)
    {
        var sql = @"SELECT U.EXIBIR_TODOS_CLIENTES_PED_SIDI ExibirTodosClientesPedidoSidi,
                    EXIBIR_TODAS_COND_PAGTO_SID_MOB ExibirTodasCondPagtos,
                    FABRICA,
                    EXIBIR_MARCA_VENDAS_WEB ExibirMarcaVendasWeb,
                    EXIBIR_TODOS_PEDIDOS_SIDIWEB ExibirTodosPedidosSidiWeb,
                    EXIBIR_SOLADO_PALMILHA_SIDIWEB ExibirSoladoPalmilhaSidiWeb,
                    EXIBIR_OBS_ITEM_SIDIWEB ExibirObsItemSidiWeb
                    FROM USUARIO U WHERE U.COD_USUARIO = @UsuarioCodigo";

        var parameters = new { query.UsuarioCodigo };
        return (await conexao.QueryAsync<UsuarioModel>(sql, parameters)).First();
    }
}

public record UsuarioQuery : IRequest<UsuarioModel>
{
    public required int UsuarioCodigo { get; set; }
}