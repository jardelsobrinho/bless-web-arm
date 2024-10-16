using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.ControleSistema;

public class ControleSistemaHandler(IDbConnection conexao) : IRequestHandler<ControleSistemaQuery, ControleSistemaModel>
{
    public async Task<ControleSistemaModel> Handle(ControleSistemaQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder();
        sql.AppendSql("SELECT USAR_PRAZO_MEDIO UsarPrazoMedio, GRADE_ESPECIAL_PEDIDO GradeEspecialPedido,");
        sql.AppendSql("FABRICA_PADRAO FabricaPadrao, CGC EmpresaCnpj, EXIBIR_PECA ExibirPeca");

        sql.AppendSql("FROM CONTROLE_SISTEMA");

        var controleSistema = await conexao.QueryAsync<ControleSistemaModel>(sql.ToString());

        return controleSistema.First();
    }
}

public record ControleSistemaQuery : IRequest<ControleSistemaModel>;
