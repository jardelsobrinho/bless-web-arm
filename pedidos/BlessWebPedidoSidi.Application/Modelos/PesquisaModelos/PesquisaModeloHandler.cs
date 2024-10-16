using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.Modelos.RetornaPreco;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Shared.GeraCaminhoImagem;
using BlessWebPedidoSidi.Application.TabelaPreco.ValidaTabelaRepresentante;
using BlessWebPedidoSidi.Application.Usuario;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Modelos.PesquisaModelos;

public class PesquisaModeloHandler(IMediator mediator, IDbConnection conexao) : IRequestHandler<PesquisaModeloQuery, IPaginacao<ModeloPesquisaModel>>
{
    private async Task ValidaDadosAsync(PesquisaModeloQuery query)
    {
        if (query.CondicaoPagamentoCodigo > 0 && query.TabelaPrecoCodigo == 0)
            throw new BadHttpRequestException("PMH02 - Informa a tabela de preço antes de informar a condição de pagamento");

        if (query.TabelaPrecoCodigo > 0)
        {
            var validaTabelaRepresentanteQuery = new ValidaTabelaRepresentanteQuery()
            {
                TabelaPrecoCodigo = query.TabelaPrecoCodigo,
                RepresentanteCnpj = query.RepresentanteCnpj
            };

            var tabelaValida = await mediator.Send(validaTabelaRepresentanteQuery);
            if (!tabelaValida)
                throw new BadHttpRequestException("PMH01 - Tabela de preço inválida para esse Representante");
        }
    }
    public async Task<IPaginacao<ModeloPesquisaModel>> Handle(PesquisaModeloQuery query, CancellationToken cancellationToken)
    {
        await ValidaDadosAsync(query);

        var controleSistema = await mediator.Send(new ControleSistemaQuery(), cancellationToken);

        var sqlSelect = new StringBuilder("SELECT M.MODELO CODIGO, M.DESCRICAO, L.DESCRICAO REFERENCIA, M.FK_LINHA ReferenciaCodigo,");

        var campoTamanho = "LC.TAMANHO";
        if (controleSistema.GradeEspecialPedido == "S")
            campoTamanho = "IIF (LC.GRADE_ESPECIAL IS NULL, LC.TAMANHO, LC.GRADE_ESPECIAL) ";

        sqlSelect.AppendSql($"(SELECT FIRST 1 {campoTamanho} FROM LINHA_COORDENACAO LC WHERE LC.CODIGO_LINHA = L.CODIGO ORDER BY LC.TAMANHO) TamanhoInicial,");
        sqlSelect.AppendSql($"(SELECT FIRST 1 {campoTamanho} FROM LINHA_COORDENACAO LC WHERE LC.CODIGO_LINHA = L.CODIGO ORDER BY LC.TAMANHO DESC) TamanhoFinal,");
        sqlSelect.AppendSql("0 PRECO");

        var sqlFrom = new StringBuilder();
        var filtros = new Dictionary<string, object>();

        if (query.TabelaPrecoCodigo > 0)
        {
            sqlFrom.AppendSql("FROM (SELECT TP.CODIGO_TABELA_HD, TP.CODIGO_MODELO FROM TABELA_PRECOS TP GROUP BY TP.CODIGO_TABELA_HD, TP.CODIGO_MODELO) TP");
            sqlFrom.AppendSql("INNER JOIN MODELOS M ON TP.CODIGO_MODELO = M.MODELO");
            sqlFrom.AppendSql("INNER JOIN LINHA L ON L.CODIGO = M.FK_LINHA");
            sqlFrom.AppendSql("WHERE TP.CODIGO_TABELA_HD = @TABELA_PRECO_CODIGO");
            filtros.Add("@TABELA_PRECO_CODIGO", query.TabelaPrecoCodigo);
        }
        else
        {
            sqlFrom.AppendSql("FROM MODELOS M");
            sqlFrom.AppendSql("INNER JOIN LINHA L ON L.CODIGO = M.FK_LINHA");
            sqlFrom.AppendSql("WHERE 1 = 1");
        }

        sqlFrom.AppendSql("AND M.ATIVA = 'T' AND M.BLOQ_PED_SIDI = 'S'");
        sqlFrom.AppendSql("AND (SELECT COUNT(*) FROM FICHA_TECNICA_HD F WHERE F.FK_MODELO = M.MODELO AND F.BLOQ_PED_SIDI = 'S') > 0");

        if (query.ModeloCodigo > 0)
        {
            sqlFrom.AppendSql("AND M.MODELO = @MODELO_CODIGO");
            filtros.Add("@MODELO_CODIGO", query.ModeloCodigo);
        }

        if (!query.ReferenciaOuDescricao.IsNullOrEmpty())
        {
            sqlFrom.AppendSql("AND (UPPER(TRIM(M.DESCRICAO)) LIKE @REFERENCIA_DESCRICAO OR UPPER(L.DESCRICAO) LIKE @REFERENCIA_DESCRICAO)");
            filtros.Add("@REFERENCIA_DESCRICAO", "%" + query.ReferenciaOuDescricao.ToUpper() + "%");
        }

        var sqlOrderBy = "ORDER BY M.DESCRICAO";

        var paginacao = await Paginacao<ModeloPesquisaModel>.CriarPaginacaoAsync(sqlSelect.ToString(), sqlFrom.ToString(), sqlOrderBy, filtros, query.Pagina, query.RegistrosPorPagina, conexao);

        foreach (var modelo in paginacao.Registros)
        {
            var sqlCodigoVersao = "SELECT FIRST 1 F.VERSAO FROM FICHA_TECNICA_HD F WHERE F.FK_MODELO = @CODIGO AND F.BLOQ_PED_SIDI = 'S' ORDER BY F.VERSAO";
            var paramCodigoVersao = new { modelo.Codigo };
            var codigoVersao = (await conexao.QueryAsync<int>(sqlCodigoVersao, paramCodigoVersao)).FirstOrDefault();

            var imagemCommand = new GeraCaminhoImagemCommand()
            {
                CorCodigo = codigoVersao,
                ModeloCodigo = modelo.Codigo
            };
            modelo.Imagem = await mediator.Send(imagemCommand, cancellationToken);

            if (query.TabelaPrecoCodigo > 0 && query.CondicaoPagamentoCodigo > 0)
            {
                var retornaPrecoQuery = new RetornaPrecoQuery()
                {
                    CondicaoPagamentoCodigo = query.CondicaoPagamentoCodigo,
                    ModeloCodigo = modelo.Codigo,
                    TabelaPrecoCodigo = query.TabelaPrecoCodigo,
                    VersaoCodigo = codigoVersao
                };

                modelo.Preco = await mediator.Send(retornaPrecoQuery, cancellationToken);

            }

            var sqlOutrasVersao = "SELECT F.VERSAO FROM FICHA_TECNICA_HD F " +
                                  "WHERE F.FK_MODELO = @CODIGO AND F.VERSAO <> @CodigoVersao AND F.BLOQ_PED_SIDI = 'S' ORDER BY F.VERSAO";

            var paramOutrasCores = new { modelo.Codigo, codigoVersao };
            var outrasCores = (await conexao.QueryAsync<string>(sqlOutrasVersao, paramOutrasCores)).ToList();

            foreach (var oc in outrasCores)
            {
                var imagemOutraCorCommand = new GeraCaminhoImagemCommand()
                {
                    CorCodigo = int.Parse(oc),
                    ModeloCodigo = modelo.Codigo
                };
                var imagemOutraCor = await mediator.Send(imagemOutraCorCommand, cancellationToken);

                //var imagemOutraCor = $"https://app-sidimobile.s3.sa-east-1.amazonaws.com/img-colecoes/{controleSistemaPedido.PastaAwsS3}/{modelo.Codigo}-{oc}/1.jpg";
                var outraCor = new ModeloCoresPesquisaModel() { Imagem = imagemOutraCor };
                modelo.OutrasCores.Add(outraCor);
            }
        }
        return paginacao;
    }
}

public record PesquisaModeloQuery : IRequest<IPaginacao<ModeloPesquisaModel>>
{
    public required int TabelaPrecoCodigo { get; set; }
    public required int CondicaoPagamentoCodigo { get; set; }
    public required int ModeloCodigo { get; set; }
    public required string ReferenciaOuDescricao { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int Pagina { get; set; }
    public required int RegistrosPorPagina { get; set; }
}
