using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.PesquisaOrcamentos;

public class PesquisaOrcamentoHandler(IDbConnection conexao) : IRequestHandler<PesquisaOrcamentosQuery, IList<PesquisaOrcamentoModel>>
{
    public async Task<IList<PesquisaOrcamentoModel>> Handle(PesquisaOrcamentosQuery request, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("select wo.id, wo.uuid, wo.data_criacao DataCriacao, wo.cliente_cnpj_cpf ClienteCnpjCpf,");
        sql.AppendSql("wo.valor_total ValorTotal, wo.status, wo.cliente_nome ClienteNome, wo.DATA_ENTREGA DataEntrega");

        if (request.Status == EOrcamentoStatus.Finalizado)
            sql.AppendSql(", p.NUMERO NumeroPedidoErp, sp.DESCRICAO SituacaoPedidoErp");

        sql.AppendSql("from web_orcamento wo");

        if (request.Status == EOrcamentoStatus.Finalizado)
        {
            sql.AppendSql("left join pedido p on p.NUMERO_PEDIDO_MARKETPLACE = wo.ID");
            sql.AppendSql("left join situacao_producao sp on sp.CODIGO = p.SITUACAO");
        }

        var filtros = new Dictionary<string, object>();

        sql.AppendSql("where status = @Status");

        if (request.PedidoDataEmissao != null)
        {
            var dataEmissao = request.PedidoDataEmissao.Value;
            var dataInicial = new DateTime(dataEmissao.Year, dataEmissao.Month, dataEmissao.Day, 0, 0, 0, 0);
            var dataFinal = new DateTime(dataEmissao.Year, dataEmissao.Month, dataEmissao.Day, 23, 59, 59, 999);

            sql.AppendSql(" AND wo.DATA_CRIACAO >= @DATA_INICIAL");
            sql.AppendSql(" AND wo.DATA_CRIACAO <= @DATA_FINAL");
            filtros.Add("@DATA_INICIAL", dataInicial);
            filtros.Add("@DATA_FINAL", dataFinal);
        }

        if (request.PedidoNumeroOuClienteNome != "")
        {
            sql.AppendSql($"and (wo.cliente_nome like '%{request.PedidoNumeroOuClienteNome}%'");

            if (int.TryParse(request.PedidoNumeroOuClienteNome, out int numeroPedido))
            {
                sql.AppendSql($" or p.NUMERO = {numeroPedido}");
            }
            sql.AppendSql(")");
        }

        sql.AppendSql("AND wo.USUARIO_CODIGO = @COD_USUARIO");
        filtros.Add("@COD_USUARIO", request.UsuarioCodigo);

        sql.AppendSql("order by wo.DATA_CRIACAO desc");
        filtros.Add("@STATUS", request.Status.ToString());

        var parameters = new DynamicParameters(filtros);
        var consulta = await conexao.QueryAsync<PesquisaOrcamentoModel>(sql.ToString(), parameters);
        return consulta.ToList();
    }
}

public record PesquisaOrcamentosQuery : IRequest<IList<PesquisaOrcamentoModel>>
{
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required string PedidoNumeroOuClienteNome { get; set; }
    public DateTime? PedidoDataEmissao { get; set; }
    public required EOrcamentoStatus Status { get; set; }
}