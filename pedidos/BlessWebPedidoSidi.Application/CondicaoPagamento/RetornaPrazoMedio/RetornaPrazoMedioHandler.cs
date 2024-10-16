using Dapper;
using MediatR;
using System.Data;

namespace BlessWebPedidoSidi.Application.CondicaoPagamento.RetornaPrazoMedio;

public class RetornaPrazoMedioHandler(IDbConnection conexao) : IRequestHandler<RetornaPrazoMedioQuery, int>
{
    private readonly IDbConnection _conexao = conexao;

    public async Task<int> Handle(RetornaPrazoMedioQuery query, CancellationToken cancellationToken)
    {
        var sql = "SELECT COALESCE(C.PRAZO_MEDIO, 0) FROM CONDICAO_PAGAMENTO C WHERE C.CODIGO = @CondicaoPagamentoCodigo";
        var parametros = new { query.CondicaoPagamentoCodigo };
        var prazoMedio = (await _conexao.QueryAsync<int>(sql, parametros)).FirstOrDefault();
        return prazoMedio;
    }
}

public record RetornaPrazoMedioQuery : IRequest<int>
{
    public int CondicaoPagamentoCodigo { get; set; }
}