using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Cliente.RetornaRepresentante;

public record RetornarDadosRepresentanteQuery : IRequest<RepresentanteModel>
{
    public required string RepresentanteCNPJ { get; set; }
}

public class RetornarDadosRepresentanteHandler(IDbConnection conexao) : IRequestHandler<RetornarDadosRepresentanteQuery, RepresentanteModel>
{
    public async Task<RepresentanteModel> Handle(RetornarDadosRepresentanteQuery query, CancellationToken cancellationToken)
    {
        List<RepresentanteModel> listaRepresentante = await RetornaRepresentantes(query, cancellationToken);
        return listaRepresentante.First();
    }

    private async Task<List<RepresentanteModel>> RetornaRepresentantes(RetornarDadosRepresentanteQuery query, CancellationToken cancellationToken)
    {
        var sql = new StringBuilder("SELECT 0 ClienteId, F.CGC CnpjCpf, F.RAZAO_SOCIAL RazaoSocial, F.NOME_FANTASIA NomeFantasia, ");
        sql.AppendSql("F.FONE1 Telefones, 'ERP' Origem, BLOQUEADO Bloqueado, E_MAIL ContatoEmail");
        sql.AppendSql("FROM FORNECEDOR F ");
        sql.AppendSql("WHERE F.REPRESENTANTE = 'T' ");

        var filtros = new Dictionary<string, object>();
        sql.AppendSql("AND F.CGC = @CNPJ_CPF");
        filtros.Add("@CNPJ_CPF", query.RepresentanteCNPJ);

        var parametros = new DynamicParameters(filtros);

        return (await conexao.QueryAsync<RepresentanteModel>(sql.ToString(), parametros)).ToList();
    }
}
