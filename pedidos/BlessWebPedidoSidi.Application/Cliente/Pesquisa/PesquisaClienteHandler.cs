using BlessWebPedidoSidi.Application.Usuario;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Cliente.Pesquisa;

public class PesquisaClienteHandler(IMediator mediator, IDbConnection conexao) : IRequestHandler<PesquisaClienteQuery, Paginacao<ClienteModel>>
{
    public async Task<Paginacao<ClienteModel>> Handle(PesquisaClienteQuery query, CancellationToken cancellationToken)
    {
        var (sqlClientes, filtrosClientes) = await RetornaSqlClientes(query, cancellationToken);
        var (sqlWebClientes, filtrosWebClientes) = RetornaSqlClientesWeb(query);

        var sqlCount = new StringBuilder("SELECT COUNT(*) FROM (");
        sqlCount.AppendSql(sqlClientes);
        sqlCount.AppendSql("UNION ALL");
        sqlCount.AppendSql(sqlWebClientes);
        sqlCount.AppendSql(") X");

        filtrosWebClientes.ToList().ForEach(x => {
            if (filtrosClientes.ContainsKey(x.Key))
                return;

            filtrosClientes.Add(x.Key, x.Value);
        });
        var parameters = new DynamicParameters(filtrosClientes);
        var totalRegistros = (await conexao.QueryAsync<int>(sqlCount.ToString(), parameters)).SingleOrDefault<int>();

        var pagina = query.Pagina == 0 ? 1 : query.Pagina;
        var skip = pagina == 1 ? 0 : (pagina - 1) * query.RegistrosPorPagina;
        
        var sql = new StringBuilder($"SELECT FIRST {query.RegistrosPorPagina} SKIP {skip} X.* FROM (");
        sql.AppendSql(sqlClientes);
        sql.AppendSql("UNION ALL");
        sql.AppendSql(sqlWebClientes);
        sql.AppendSql(") X");
        sql.AppendSql("ORDER BY X.RazaoSocial");

        var listaClientes = (await conexao.QueryAsync<ClienteModel>(sql.ToString(), parameters)).ToList();
        return new Paginacao<ClienteModel>(listaClientes, query.Pagina, query.RegistrosPorPagina, totalRegistros);
    }

    private static (string, Dictionary<string, object>) RetornaSqlClientesWeb(PesquisaClienteQuery query)
    {
        var sql = new StringBuilder("select C.ID CLIENTEID, C.CNPJ_CPF CNPJCPF, C.RAZAO_SOCIAL RAZAOSOCIAL, C.NOME_FANTASIA NOMEFANTASIA,");
        sql.AppendSql("CE.RUA || ', ' || CE.NUMERO ENDERECO, CE.BAIRRO, CI.NOME CIDADE, U.DESCRICAO UF, CE.CEP, coalesce(C.CELULAR_DDD, '') || coalesce(C.CELULAR_NUMERO, '') ||");
        sql.AppendSql("case");
        sql.AppendSql("when coalesce(C.CELULAR_NUMERO, '') <> '' and coalesce(C.TELEFONE_NUMERO, '') <> '' then ', ' || coalesce(C.TELEFONE_DDD, '') || coalesce(C.TELEFONE_NUMERO, '')");
        sql.AppendSql("end TELEFONES, 'WEB' ORIGEM, 'F' BLOQUEADO,");
        sql.AppendSql("C.FRETE_POR_CONTA FRETEPORCONTA, C.CONTATO_EMAIL CONTATOEMAIL");
        sql.AppendSql("from web_cliente c inner join web_cliente_endereco ce on ce.cliente_id = c.id");
        sql.AppendSql("inner join cidade ci on ci.codigo = ce.cidade_codigo");
        sql.AppendSql("inner join uf u on u.sigla_uf = ci.uf");
        sql.AppendSql("where ce.tipo = 'Principal' and c.empresa_cnpj = @EMPRESA_CNPJ and c.representante_cnpj = @REPRESETANTE_CNPJ");
        sql.AppendSql("and c.sincronizado = 'N' ");
        sql.AppendSql("and (select count(*) from fornecedor f where f.cgc = c.cnpj_cpf and " +
            "(select count(*) from FORNECEDOR_REPRESENTANTES FR where FR.CGC_FORNCEDOR = F.CGC) > 0) = 0");

        var filtros = new Dictionary<string, object>
        {
            { "EMPRESA_CNPJ", query.EmpresaCnpj },
            { "REPRESETANTE_CNPJ", query.RepresentanteCnpj }
        };

        if (query.CnpjCpf != "")
        {
            sql.AppendSql("and c.cnpj_cpf = @CNPJ_CPF");
            filtros.Add("@CNPJ_CPF", query.CnpjCpf);
        }

        if (query.CnpjOuNome != "")
        {
            sql.AppendSql("and (c.razao_social like @RAZAO_SOCIAL");

            var cnpj = query.CnpjOuNome;
            if (cnpj.Length >= 14)
                cnpj = query.CnpjOuNome[..14];

            if (cnpj.All(char.IsDigit))
            {
                if (cnpj.Length == 11 || cnpj.Length == 14)
                {
                    sql.AppendSql("or c.cnpj_cpf = @CNPJ");
                    filtros.Add("@CNPJ", cnpj);
                }
                else
                {
                    sql.AppendSql("or c.cnpj_cpf like @CNPJ");
                    filtros.Add("@CNPJ", query.CnpjOuNome + "%");
                }
            }
            sql.AppendSql(")");

            filtros.Add("@RAZAO_SOCIAL", query.CnpjOuNome.ToUpper() + "%");
        }

        return (sql.ToString(), filtros);
    }

    private async Task<(string, Dictionary<string, object>)> RetornaSqlClientes(PesquisaClienteQuery query, CancellationToken cancellationToken)
    {
        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = query.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery, cancellationToken);

        var sql = new StringBuilder("SELECT 0 ClienteId, F.CGC CnpjCpf, F.RAZAO_SOCIAL RazaoSocial, F.NOME_FANTASIA NomeFantasia, F.ENDERECO, ");
        sql.AppendSql("F.BAIRRO, F.CIDADE, F.SIGLA_UF UF, F.CEP, F.FONE1 Telefones, 'ERP' Origem, BLOQUEADO Bloqueado, ");
        sql.AppendSql("IIF(F.FRETE_POR_CONTA IN ('E', 'R'), 'CIF', 'FOB') FretePorConta, E_MAIL ContatoEmail");
        sql.AppendSql("FROM FORNECEDOR F ");

        sql.AppendSql("LEFT JOIN FORNECEDOR_REPRESENTANTES FR ON FR.CGC_FORNCEDOR = F.CGC ");
        sql.AppendSql("WHERE F.CLIENTE = 'T' AND F.BLOQUEADO = 'F'");

        var filtros = new Dictionary<string, object>();

        if (query.ApenasClientesDoRepresentante)
        {
            if (usuario.ExibirTodosClientesPedidoSidi != "T")
            {
                sql.AppendSql("AND (FR.CGC_REPRESENTANTE = @REPRESENTANTE_CNPJ OR F.REPRESENTANTE_CLIENTE = @REPRESENTANTE_CNPJ) ");
                filtros.Add("@REPRESENTANTE_CNPJ", query.RepresentanteCnpj);
            }
        }

        if (query.CnpjCpf != "")
        {
            sql.AppendSql("AND F.CGC = @CNPJ_CPF");
            filtros.Add("@CNPJ_CPF", query.CnpjCpf);
        }

        if (query.CnpjOuNome != "")
        {
            sql.AppendSql("AND (F.RAZAO_SOCIAL LIKE @RAZAO_SOCIAL");

            var cnpj = query.CnpjOuNome;
            if (cnpj.Length >= 14)
                cnpj = query.CnpjOuNome[..14];

            if (cnpj.All(char.IsDigit))
            {
                if (cnpj.Length == 14 || cnpj.Length == 11)
                {
                    sql.AppendSql("OR F.CGC = @CNPJ");
                    filtros.Add("@CNPJ", cnpj);
                }
                else
                {
                    sql.AppendSql("OR F.CGC LIKE @CNPJ");
                    filtros.Add("@CNPJ", query.CnpjOuNome + "%");
                }
            }
            sql.AppendSql(")");

            filtros.Add("@RAZAO_SOCIAL", query.CnpjOuNome + "%");
        }

        return (sql.ToString(), filtros);
    }
}
public record PesquisaClienteQuery : IRequest<Paginacao<ClienteModel>>
{
    public required string CnpjOuNome { get; set; }
    public required string CnpjCpf { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required bool ApenasClientesDoRepresentante { get; set; }
    public required int Pagina { get; set; } = 1;
    public required int RegistrosPorPagina { get; set; } = 10;
}