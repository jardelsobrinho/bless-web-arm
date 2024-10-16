using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaStatusOrcamento;
public class RetornaStatusOrcamentoHandler(IDbConnection conexao) : IRequestHandler<RetornaStatusOrcamentoQuery, string>
{
    private readonly IDbConnection _conexao = conexao;

    public async Task<string> Handle(RetornaStatusOrcamentoQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("SELECT O.STATUS Status FROM WEB_ORCAMENTO O WHERE O.UUID = @UUID");
        sql.AppendSql("AND O.REPRESENTANTE_CNPJ = @RepresentanteCnpj AND O.USUARIO_CODIGO = @UsuarioCodigo");

        var param = new
        {
            UUID = query.Uuid,
            query.RepresentanteCnpj,
            query.UsuarioCodigo
        };

        var consulta = (await _conexao.QueryAsync<string>(sql.ToString(), param)).FirstOrDefault() ?? "";
        return consulta;
    }
}

public record RetornaStatusOrcamentoQuery : IRequest<string>
{
    public required string Uuid { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}