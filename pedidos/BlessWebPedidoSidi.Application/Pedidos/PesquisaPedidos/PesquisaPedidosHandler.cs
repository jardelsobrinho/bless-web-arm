using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Usuario;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Pedidos.PesquisaPedidos;

public class PesquisaPedidosHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<PesquisaPedidosQuery, IPaginacao<PesquisaPedidosModel>>
{
    public async Task<IPaginacao<PesquisaPedidosModel>> Handle(PesquisaPedidosQuery query, CancellationToken cancellationToken)
    {
        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = query.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery, cancellationToken);

        var filtros = new Dictionary<string, object>
        {
            { "REPRESENTANTE_CNPJ", query.RepresentanteCnpj }
        };

        if (usuario.ExibirTodosPedidosSidiWeb != "S")
        {
            filtros.Add("USUARIO_CODIGO", query.UsuarioCodigo);
        }
      
        var sqlSelect = new StringBuilder("SELECT W.UUID OrcamentoUuid, P.CGC_CLIENTE ClienteCnpjCpf, C.RAZAO_SOCIAL ClienteNome,");
        sqlSelect.AppendSql("P.DATA_PEDIDO DataEmissao, P.VALOR_PEDIDO ValorTotal, W.STATUS Status, P.NUMERO Numero, SP.DESCRICAO Situacao, P.DATA_ENTREGA_SOLICITADA DataEntrega,");
        sqlSelect.AppendSql("P.ONLINE");

        var sqlFrom = new StringBuilder();

        if (usuario.ExibirTodosPedidosSidiWeb == "S")
        {
            sqlFrom.AppendSql("FROM PEDIDO P");
            sqlFrom.AppendSql("LEFT JOIN WEB_ORCAMENTO W ON P.NUMERO_PEDIDO_MARKETPLACE = W.ID");
        } else
        {
            sqlFrom.AppendSql("FROM WEB_ORCAMENTO W");
            sqlFrom.AppendSql("INNER JOIN PEDIDO P ON P.NUMERO_PEDIDO_MARKETPLACE = W.ID");
        }

        sqlFrom.AppendSql("INNER JOIN FORNECEDOR C ON C.CGC = P.CGC_CLIENTE");
        sqlFrom.AppendSql("INNER JOIN SITUACAO_PRODUCAO SP ON SP.CODIGO = P.SITUACAO");


        if (usuario.ExibirTodosPedidosSidiWeb == "S")
        {
            sqlFrom.AppendSql("WHERE P.CGC_REPRESENTANTE = @REPRESENTANTE_CNPJ");
        } else
        {
            sqlFrom.AppendSql("WHERE W.REPRESENTANTE_CNPJ = @REPRESENTANTE_CNPJ AND W.USUARIO_CODIGO = @USUARIO_CODIGO AND W.STATUS = 'Finalizado'");
        }           

        if (query.NumeroOuNomeCliente != "")
        {
            sqlFrom.AppendSql("AND (C.RAZAO_SOCIAL LIKE @RAZAO_SOCIAL OR C.NOME_FANTASIA = @NOME_FANTASIA");
            filtros.Add("RAZAO_SOCIAL", query.NumeroOuNomeCliente + "%");
            filtros.Add("NOME_FANTASIA", query.NumeroOuNomeCliente + "%");
            if (query.NumeroOuNomeCliente.All(char.IsDigit))
            {
                sqlFrom.AppendSql("OR P.NUMERO = @NUMERO");
                filtros.Add("@NUMERO", query.NumeroOuNomeCliente);
            }
            sqlFrom.AppendSql(")");
        }

        if (query.DataEmissao != null)
        {
            var dataEmissao = query.DataEmissao.Value;
            var dataInicial = new DateTime(dataEmissao.Year, dataEmissao.Month, dataEmissao.Day, 0, 0, 0, 0);
            var dataFinal = new DateTime(dataEmissao.Year, dataEmissao.Month, dataEmissao.Day, 23, 59, 59, 999);

            sqlFrom.AppendSql(" AND P.DATA_PEDIDO >= @DATA_INICIAL");
            sqlFrom.AppendSql(" AND P.DATA_PEDIDO <= @DATA_FINAL");
            filtros.Add("@DATA_INICIAL", dataInicial);
            filtros.Add("@DATA_FINAL", dataFinal);
        }

        var sqlOrderBy = "ORDER BY P.DATA_PEDIDO DESC, P.NUMERO";

        return await Paginacao<PesquisaPedidosModel>.CriarPaginacaoAsync(sqlSelect.ToString(), sqlFrom.ToString(), sqlOrderBy, filtros, query.Pagina, query.RegistrosPorPagina, conexao);
    }
}

public record PesquisaPedidosQuery : IRequest<IPaginacao<PesquisaPedidosModel>>
{
    public required string NumeroOuNomeCliente { get; set; }
    public DateTime? DataEmissao { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required int Pagina { get; set; }
    public required int RegistrosPorPagina { get; set; }
}
