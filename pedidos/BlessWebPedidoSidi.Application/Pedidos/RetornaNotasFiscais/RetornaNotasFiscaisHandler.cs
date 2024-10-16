using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Pedidos.RetornaNotasFiscais;

public class RetornaNotasFiscaisHandler(IDbConnection conexao) : IRequestHandler<RetornaNotasFiscaisQuery, IList<RetornaNotaFiscalModel>>
{
    public async Task<IList<RetornaNotaFiscalModel>> Handle(RetornaNotasFiscaisQuery request, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("SELECT PE.NOTA_FISCAL Numero, PE.SERIE_NF Serie, PE.SEQUENCIA Entrega");
        sql.AppendSql("FROM PEDIDO_ENTREGA_ITEM PEI");
        sql.AppendSql("INNER JOIN PEDIDO_ENTREGA PE ON PE.NUMERO = PEI.NUMERO AND PE.SEQUENCIA = PEI.SEQUENCIA");
        sql.AppendSql("WHERE PEI.NUMERO = @PEDIDO AND PEI.SEQ_ITEM = @SEQ_ITEM");

        var filtros = new DynamicParameters();
        filtros.Add("@PEDIDO", request.Pedido);
        filtros.Add("@SEQ_ITEM", request.Sequencia);

        var notasPorEntrega = (await conexao.QueryAsync<RetornaNotaFiscalModel>(sql.ToString(), filtros)).ToList();
        if (notasPorEntrega.Count > 0)
        {
           return notasPorEntrega;
        }

        sql.Clear();
        sql.AppendSql("SELECT PI.NOTA_FISCAL Numero, PI.SERIE_NF Serie, -1 Entrega");
        sql.AppendSql("FROM PEDIDO_ITEM PI");
        sql.AppendSql("WHERE PI.NUMERO = @PEDIDO AND PI.SEQUENCIA = @SEQUENCIA");

        var filtrosPedido = new DynamicParameters();
        filtrosPedido.Add("@PEDIDO", request.Pedido);
        filtrosPedido.Add("@SEQUENCIA", request.Sequencia);

        var notasPorPedido =  (await conexao.QueryAsync<RetornaNotaFiscalModel>(sql.ToString(), filtrosPedido)).ToList();
        if (notasPorPedido.Count > 0 && notasPorPedido.First().Numero > 0)
        {
            return notasPorPedido;
        }

        return [];
    }
}

public record RetornaNotasFiscaisQuery : IRequest<IList<RetornaNotaFiscalModel>>
{
    public int Pedido { get; init; }
    public int Sequencia { get; init; }
}
