using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.AcompanhamentoEmpresa;

public class RetornaDadosOrcamentoEmpresaHandle(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaDadosOrcamentoEmpresaQuery, DadosEmpresaModel>
{
    public async Task<DadosEmpresaModel> Handle(RetornaDadosOrcamentoEmpresaQuery query, CancellationToken cancellationToken)
    {
        var controleSistemaQuery = new ControleSistemaQuery();
        var controleSistema = await mediator.Send(controleSistemaQuery, cancellationToken);

        var dataInicial = new DateTime(query.DataInicial!.Value.Year, query.DataInicial.Value.Month, query.DataInicial.Value.Day, 0, 0, 0, 0);
        var dataFinal = new DateTime(query.DataFinal!.Value.Year, query.DataFinal.Value.Month, query.DataFinal.Value.Day, 23, 59, 59, 999);

        var sqlPesquisaQtePedido = new StringBuilder("SELECT Count(P.NUMERO) QuantidadePedido FROM PEDIDO P WHERE P.ONLINE = 'W'");
        var filtros = new Dictionary<string, object>();

        if (query.DataInicial != DateTime.MinValue)
        {
            sqlPesquisaQtePedido.AppendSql("AND P.DATA_CADASTRO >= @DATA_INICIO");
            filtros.Add("@DATA_INICIO", dataInicial);
        }

        if (query.DataFinal != DateTime.MinValue)
        {
            sqlPesquisaQtePedido.AppendSql("AND P.DATA_CADASTRO <= @DATA_FIM");
            filtros.Add("@DATA_FIM", dataFinal);
        }

        var parametros = new DynamicParameters(filtros);
        var quantidadePedido = (await conexao.QueryAsync<int>(sqlPesquisaQtePedido.ToString(), parametros)).FirstOrDefault();

        var sqlPesquisaEmpresa = $"SELECT DESCRICAO Empresa FROM FABRICA WHERE CODIGO = {controleSistema.FabricaPadrao}";
        var descricaoEmpresa = (await conexao.QueryAsync<string>(sqlPesquisaEmpresa)).FirstOrDefault();

        var dadosEmpresa = new DadosEmpresaModel()
        {
            Cnpj = controleSistema.EmpresaCnpj!,
            Empresa = descricaoEmpresa!,
            QuantidadePedido = quantidadePedido,
        };
        return dadosEmpresa;
    }
}

public record RetornaDadosOrcamentoEmpresaQuery : IRequest<DadosEmpresaModel>
{
    public DateTime? DataInicial { get; init; }
    public DateTime? DataFinal { get; init; }
}