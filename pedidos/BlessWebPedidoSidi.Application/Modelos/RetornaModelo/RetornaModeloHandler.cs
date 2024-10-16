using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.EstoqueAcabado;
using BlessWebPedidoSidi.Application.Modelos.PesquisaModelos;
using BlessWebPedidoSidi.Application.Modelos.RetornaModelo.Models;
using BlessWebPedidoSidi.Application.Modelos.RetornaPreco;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Shared.GeraCaminhoImagem;
using BlessWebPedidoSidi.Application.Tamanhos;
using BlessWebPedidoSidi.Application.Usuario;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Modelos.RetornaModelo;

public class RetornaModeloHandler(IMediator mediator, IDbConnection conexao) : IRequestHandler<RetornaModeloQuery, ModeloModel>
{
    public async Task<ModeloModel> Handle(RetornaModeloQuery query, CancellationToken cancellationToken)
    {
        var queryModelo = new PesquisaModeloQuery
        {
            TabelaPrecoCodigo = query.TabelaPrecoCodigo,
            CondicaoPagamentoCodigo = query.CondicaoPagamentoCodigo,
            ModeloCodigo = query.ModeloCodigo,
            ReferenciaOuDescricao = "",
            RepresentanteCnpj = query.RepresentanteCnpj,
            Pagina = 1,
            RegistrosPorPagina = 1
        };

        var modeloPesquisaModel = await mediator.Send(queryModelo, cancellationToken);

        if (modeloPesquisaModel.Registros.Count == 0)
            throw new BadHttpRequestException($"RMH01 - Nenhum modelo foi encontrado no código {queryModelo.ModeloCodigo}");

        var modeloModel = modeloPesquisaModel.Registros[0];

        var pesquisaControleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(pesquisaControleSistemaPedidoQuery, cancellationToken);

        var usuario = await mediator.Send(new UsuarioQuery() { UsuarioCodigo = query.UsuarioCodigo }, cancellationToken);

        var modelo = new ModeloModel()
        {
            Descricao = modeloModel.Descricao,
            Imagem = modeloModel.Imagem,
            TamanhoFinal = modeloModel.TamanhoFinal,
            Codigo = modeloModel.Codigo,
            ReferenciaCodigo = modeloModel.ReferenciaCodigo,
            Referencia = modeloModel.Referencia,
            TamanhoInicial = modeloModel.TamanhoInicial,
            ExibirObservacaoItem = usuario.ExibirObsItemSidiWeb == "S",
            ValidaEstoque = controleSistemaPedido.ValidaEstoqueDisponivel == "T" ||
                controleSistemaPedido.ValidaEstoqueAcabadoSibMobile == "T"
        };

        var sqlCores = new StringBuilder("SELECT VERSAO CODIGO, NOME, CODIGO_MARCA MarcaCodigo,");
        sqlCores.AppendSql("SOLADO_ITEM_ESTOQUE SoladoCodigo, PALMILHA_ITEM_ESTOQUE PalmilhaCodigo");
        sqlCores.AppendSql("FROM FICHA_TECNICA_HD F WHERE F.FK_MODELO = @MODELO");
        sqlCores.AppendSql("AND F.BLOQ_PED_SIDI = 'S'");

        var filtrosCores = new DynamicParameters();
        filtrosCores.Add("@MODELO", modelo.Codigo);

        var listaModeloCores = (await conexao.QueryAsync<ModeloCorModel>(sqlCores.ToString(), filtrosCores)).ToList();

        var retornaTamanhosQuery = new RetornaTamanhosQuery()
        {
            TipoCodigo = TipoCodigoRetornaTamanho.Referencia,
            Codigo = modelo.ReferenciaCodigo
        };
        var listaTamanhos = await mediator.Send(retornaTamanhosQuery, cancellationToken);

        if (query.CodigoCor != null)
        {
            listaModeloCores = listaModeloCores.Where(x => x.Codigo == query.CodigoCor).ToList();
        }

        foreach (var cor in listaModeloCores)
        {
            var retornaPrecoQuery = new RetornaPrecoQuery()
            {
                CondicaoPagamentoCodigo = query.CondicaoPagamentoCodigo,
                TabelaPrecoCodigo = query.TabelaPrecoCodigo,
                ModeloCodigo = query.ModeloCodigo,
                VersaoCodigo = cor.Codigo
            };

            var preco = await mediator.Send(retornaPrecoQuery, cancellationToken);

            if (preco == 0)
            {
                continue;
            }

            var geraCaminhoImagem = new GeraCaminhoImagemCommand()
            {
                CorCodigo = cor.Codigo,
                ModeloCodigo = modelo.Codigo
            };
            var imagem = await mediator.Send(geraCaminhoImagem, cancellationToken);

            var modeloCor = new ModeloCorModel()
            {
                Codigo = cor.Codigo,
                Nome = cor.Nome,
                Imagem = imagem,
                MarcaCodigo = cor.MarcaCodigo,
                SoladoCodigo = cor.SoladoCodigo,
                PalmilhaCodigo = cor.PalmilhaCodigo,
                Preco = preco,
            };

            IList<EstoqueAcabadoModel> listaEstoques = [];

            if (controleSistemaPedido.ValidaEstoqueAcabadoSibMobile == "T")
            {
                var estoqueAcabadoQuery = new EstoqueAcabadoQuery()
                {
                    CorCodigo = cor.Codigo,
                    ModeloCodigo = modelo.Codigo,
                    UsuarioCodigo = query.UsuarioCodigo
                };
                listaEstoques = await mediator.Send(estoqueAcabadoQuery, cancellationToken);
            }

            foreach (var tamanho in listaTamanhos)
            {
                double estoqueDoTamanhoDaCor = (from e in listaEstoques
                                                where e.TamanhoCodigo == tamanho.Codigo
                                                select e.Estoque).SingleOrDefault();

                var novoTamanho = new TamanhoModel()
                {
                    Codigo = tamanho.Codigo,
                    Descricao = tamanho.Descricao,
                    Estoque = estoqueDoTamanhoDaCor
                };

                modeloCor.Grade.Add(novoTamanho);
            }

            modelo.Cores.Add(modeloCor);
        }

        return modelo;
    }
}
public record RetornaModeloQuery : IRequest<ModeloModel>
{
    public required int ModeloCodigo { get; set; }
    public int? CodigoCor { get; set; }
    public required int TabelaPrecoCodigo { get; set; }
    public required int CondicaoPagamentoCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required string OrcamentoUuid { get; set; }
}