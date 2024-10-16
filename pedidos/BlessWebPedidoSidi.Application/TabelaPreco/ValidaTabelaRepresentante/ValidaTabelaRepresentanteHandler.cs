using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.TabelaPreco.ValidaTabelaRepresentante;

public class ValidaTabelaRepresentanteHandler(IDbConnection conexao) : IRequestHandler<ValidaTabelaRepresentanteQuery, bool>
{
    public Task<bool> Handle(ValidaTabelaRepresentanteQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder();
        sql.AppendSql(@"SELECT COUNT(*) FROM FORNECEDOR F
                        WHERE F.CGC = @RepresentanteCnpj AND F.TABELA_PRECO = @TabelaPrecoCodigo");

        var filtro = new { query.RepresentanteCnpj, query.TabelaPrecoCodigo };
        var tabelaNoRepresentante = conexao.Query<int>(sql.ToString(), filtro).First();

        if (tabelaNoRepresentante == 0)
        {
            sql.Clear();
            sql.AppendSql(@"SELECT COUNT(*)
                            FROM TABELA_PRECOS_REPRESENTANTE TPR
                            INNER JOIN TABELA_PRECOS TP ON TP.CODIGO_TABELA_HD = TPR.FK_TABELA_HD
                            WHERE TPR.FK_TABELA_HD = @TabelaPrecoCodigo AND
                                  TPR.CGC_REPRESENTANTE = @RepresentanteCnpj");

            filtro = new { query.RepresentanteCnpj, query.TabelaPrecoCodigo };
            var tabelaPrecoNoRepresentante = conexao.Query<int>(sql.ToString(), filtro).First();

            if (tabelaPrecoNoRepresentante == 0)
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}

public record ValidaTabelaRepresentanteQuery : IRequest<bool>
{
    public int TabelaPrecoCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
}