using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.ControleSistemaPedidoSidi;

public class ControleSistemaPedidoSidiHandler(IDbConnection conexao) : IRequestHandler<ControleSistemaPedidoSidiQuery, ControleSistemaPedidoSidiModel>
{
    public async Task<ControleSistemaPedidoSidiModel> Handle(ControleSistemaPedidoSidiQuery request, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("SELECT C.PREENCHE_PREVISAO_ENTREGA PreencherPrevisaoEntrega");
        sql.AppendSql("FROM CONTROLE_SISTEMA_PEDIDO_SIDI C");

        var preferencias = await conexao.QueryAsync<ControleSistemaPedidoSidiModel>(sql.ToString());
        return preferencias.First();
    }
}

public record ControleSistemaPedidoSidiQuery : IRequest<ControleSistemaPedidoSidiModel> { }
