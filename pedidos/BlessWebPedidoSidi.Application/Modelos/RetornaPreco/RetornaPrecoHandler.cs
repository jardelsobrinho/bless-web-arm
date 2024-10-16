using BlessWebPedidoSidi.Application.CondicaoPagamento.RetornaPrazoMedio;
using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Modelos.RetornaPreco;

public class RetornaPrecoHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaPrecoQuery, double>
{
    public async Task<double> Handle(RetornaPrecoQuery request, CancellationToken cancellationToken)
    {
        var controleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(controleSistemaPedidoQuery, cancellationToken);

        var controleSistemaQuery = new ControleSistemaQuery();
        var controleSistema = await mediator.Send(controleSistemaQuery, cancellationToken);

        var condicaoPagamentoCodigo = request.CondicaoPagamentoCodigo;
        if (controleSistema.UsarPrazoMedio == "T")
        {
            var retornaPrazoMedioQuery = new RetornaPrazoMedioQuery() { CondicaoPagamentoCodigo = condicaoPagamentoCodigo };
            condicaoPagamentoCodigo = await mediator.Send(retornaPrazoMedioQuery, cancellationToken);
        }

        var sqlPreco = new StringBuilder("SELECT FIRST 1 TPC.PRECO_UNITARIO");
        sqlPreco.AppendSql("FROM TABELA_PRECOS_CONDICOES TPC INNER JOIN TABELA_PRECOS T ON TPC.CODIGO_TABELA = T.CODIGO_TABELA");
        sqlPreco.AppendSql("WHERE T.CODIGO_TABELA_HD = @CODIGO_TABELA_HD AND T.CODIGO_MODELO = @MODELO AND TPC.CODIGO_PAGTO = @CONDICAO_PAGAMENTO_CODIGO");

        if (controleSistemaPedido.PermitirCoresTabelaPreco == "S")
        {
            sqlPreco.AppendSql("AND T.CODIGO_VERSAO = @VERSAO");
        }

        var filtrosPreco = new DynamicParameters();
        filtrosPreco.Add("@CODIGO_TABELA_HD", request.TabelaPrecoCodigo);
        filtrosPreco.Add("@MODELO", request.ModeloCodigo);
        filtrosPreco.Add("@CONDICAO_PAGAMENTO_CODIGO", condicaoPagamentoCodigo);

        if (controleSistemaPedido.PermitirCoresTabelaPreco == "S")
        {
            filtrosPreco.Add("@VERSAO", request.VersaoCodigo);
        }

        return (await conexao.QueryAsync<double>(sqlPreco.ToString(), filtrosPreco)).FirstOrDefault();
    }
}

public record RetornaPrecoQuery : IRequest<double>
{
    public int TabelaPrecoCodigo { get; init; }
    public int CondicaoPagamentoCodigo { get; init; }
    public int ModeloCodigo { get; init; }
    public int VersaoCodigo { get; init; }
}
