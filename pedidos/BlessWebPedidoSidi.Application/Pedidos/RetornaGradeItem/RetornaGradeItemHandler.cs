using BlessWebPedidoSidi.Application.Pedidos.RetornaDadosPedido;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Tamanhos;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Pedidos.RetornaGradeItem;

public class RetornaGradeItemHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaGradeItemQuery, List<GradeItemModel>>
{
    public async Task<List<GradeItemModel>> Handle(RetornaGradeItemQuery query, CancellationToken cancellationToken)
    {
        var sqlConsulta = new StringBuilder("SELECT P.NOTA_FISCAL, PEI.SEQ_ITEM, PG.TAMANHO_CALCADO, PG.QTE_PEDIDA, PEI.MODELO");
        sqlConsulta.AppendSql("FROM PEDIDO_ENTREGA_ITEM PEI");
        sqlConsulta.AppendSql("INNER JOIN PEDIDO_ENTREGA P ON P.NUMERO = PEI.NUMERO AND PEI.SEQUENCIA = P.SEQUENCIA");
        sqlConsulta.AppendSql("INNER JOIN PEDIDO_ENTREGA_ITEM_GRADE PG ON PG.NUMERO = PEI.NUMERO AND PG.SEQUENCIA = PEI.SEQUENCIA AND PG.SEQ_ITEM = PEI.SEQ_ITEM");
        sqlConsulta.AppendSql("WHERE PEI.NUMERO = @PEDIDO AND PEI.SEQ_ITEM = @SEQUENCIA");

        var filtros = new DynamicParameters();
        filtros.Add("@PEDIDO", query.Pedido);
        filtros.Add("@SEQUENCIA", query.Sequencia);

        var dadosConsulta = (await conexao.QueryAsync<RetornaGradeItemConsulta>(sqlConsulta.ToString(), filtros)).ToList();

        var sqlTamanhos = new StringBuilder("SELECT P.TAMANHO_CALCADO, P.QTE_PEDIDA, P.MODELO FROM PEDIDO_ITEM_GRADE P");
        sqlTamanhos.AppendSql("WHERE P.PEDIDO = @PEDIDO AND P.SEQUENCIA = @SEQUENCIA");
        sqlTamanhos.AppendSql("ORDER BY P.TAMANHO_CALCADO");

        var filtroTam = new DynamicParameters();
        filtroTam.Add("@PEDIDO", query.Pedido);
        filtroTam.Add("@SEQUENCIA", query.Sequencia);

        var dadosTamanhos = (await conexao.QueryAsync<RetornaTamanhosConsulta>(sqlTamanhos.ToString(), filtroTam)).ToList();

        var retornaTamanhosQuery = new RetornaTamanhosQuery()
        {
            TipoCodigo = TipoCodigoRetornaTamanho.Modelo,
            Codigo = dadosTamanhos.First().MODELO
        };

        var listaTamanhos = await mediator.Send(retornaTamanhosQuery, cancellationToken);

        var listaItensGrade = new List<GradeItemModel>();
        foreach (var t in dadosTamanhos)
        {
            var tamanho = (listaTamanhos.Where(x => x.Codigo == t.TAMANHO_CALCADO)).FirstOrDefault();
            var tamanhoDescricao = t.TAMANHO_CALCADO;
            if (tamanho != null)
            {
                tamanhoDescricao = tamanho.Descricao;
            }

            var grade = new GradeItemModel()
            {
                Tamanho = t.TAMANHO_CALCADO,
                TamanhoDescricao = tamanhoDescricao,
                Quantidade = t.QTE_PEDIDA,
                Faturado = 0,
                EmAberto = t.QTE_PEDIDA
            };
            listaItensGrade.Add(grade);
        }

        foreach (var c in dadosConsulta)
        {
            var item = listaItensGrade.Where(x => x.Tamanho == c.TAMANHO_CALCADO).FirstOrDefault();

            if (c.NOTA_FISCAL != null && c.NOTA_FISCAL != "")
            {
                if (item != null)
                {
                    item.Faturado = c.QTE_PEDIDA;
                    item.EmAberto -= c.QTE_PEDIDA;
                }
            }
        }

        return listaItensGrade;
    }
}

public record RetornaGradeItemQuery : IRequest<List<GradeItemModel>>
{
    public int Pedido { get; init; }
    public int Sequencia { get; init; }
}
