using BlessWebPedidoSidi.Application.Shared.GeraCaminhoImagem;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;
using BlessWebPedidoSidi.Application.Usuario;

namespace BlessWebPedidoSidi.Application.Pedidos.RetornaDadosPedido;

public record RetornaDadosPedidoQuery : IRequest<RetornaDadosPedidoModel>
{
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required int Pedido { get; set; }
}
public class RetornaDadosPedidoHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaDadosPedidoQuery, RetornaDadosPedidoModel>
{
    public async Task<RetornaDadosPedidoModel> Handle(RetornaDadosPedidoQuery query, CancellationToken cancellationToken)
    {
        var usuarioQuery = new UsuarioQuery() { UsuarioCodigo = query.UsuarioCodigo };
        var usuario = await mediator.Send(usuarioQuery, cancellationToken);

        var sqlPedido = new StringBuilder("SELECT P.NUMERO Numero, P.VALOR_PEDIDO ValorPedido,");
        sqlPedido.AppendSql("F.RAZAO_SOCIAL ClienteRazaoSocial, P.CGC_CLIENTE ClienteCnpjCpf,");
        sqlPedido.AppendSql("T.NOME TabelaPrecoNome, CP.DESCRICAO CondicaoPagamentoDescricao,");
        sqlPedido.AppendSql("P.VALOR_PEDIDO ValorTotal, P.DATA_PEDIDO DataEmissao,");
        sqlPedido.AppendSql("SP.DESCRICAO Situacao");
        sqlPedido.AppendSql("FROM PEDIDO P");
        sqlPedido.AppendSql("INNER JOIN FORNECEDOR F ON F.CGC = P.CGC_CLIENTE");
        sqlPedido.AppendSql("INNER JOIN TABELA_PRECOS_HD T ON T.CODIGO = P.TABELA_PRECO");
        sqlPedido.AppendSql("INNER JOIN CONDICAO_PAGAMENTO CP ON CP.CODIGO = P.FORMA_PAGAMENTO");
        sqlPedido.AppendSql("INNER JOIN SITUACAO_PRODUCAO SP ON SP.CODIGO = P.SITUACAO");
        sqlPedido.AppendSql("WHERE P.NUMERO = @PEDIDO AND P.CGC_REPRESENTANTE = @REPRESENTANTE_CNPJ");

        var filtros = new DynamicParameters();
        filtros.Add("@PEDIDO", query.Pedido);
        filtros.Add("@REPRESENTANTE_CNPJ", query.RepresentanteCnpj);

        var pedidoModel = (await conexao.QueryAsync<RetornaDadosPedidoModel>(sqlPedido.ToString(), filtros))
                .FirstOrDefault() ?? throw new BadHttpRequestException($"RTPH01 - Pedido não encontrado com o número {query.Pedido}");

        var sqlItens = new StringBuilder("SELECT PI.MODELO Modelo, L.DESCRICAO Referencia, M.DESCRICAO ModeloDescricao,");
        sqlItens.AppendSql("FT.NOME CorNome, PI.TOTAL_PARES TotalPares, PI.PRECO_UNIT PrecoUnitario, PI.VERSAO Versao,");
        sqlItens.AppendSql("pi.MARCA MarcaCodigo, MC.NOME_MARCA MarcaNome, PI.SEQUENCIA Sequencia");
        sqlItens.AppendSql("FROM PEDIDO_ITEM PI");
        sqlItens.AppendSql("INNER JOIN MODELOS M ON M.MODELO = PI.MODELO");
        sqlItens.AppendSql("INNER JOIN LINHA L ON L.CODIGO = M.FK_LINHA");
        sqlItens.AppendSql("INNER JOIN FICHA_TECNICA_HD FT ON FT.FK_MODELO = PI.MODELO AND FT.VERSAO = PI.VERSAO");
        sqlItens.AppendSql("LEFT JOIN MARCA MC ON MC.CODIGO = PI.MARCA");
        sqlItens.AppendSql("WHERE PI.NUMERO = @PEDIDO_NUMERO");

        var filtrosItens = new DynamicParameters();
        filtrosItens.Add("@PEDIDO_NUMERO", pedidoModel.Numero);

        var itensModel = (await conexao.QueryAsync<RetornaDadosPedidoItemModel>(sqlItens.ToString(), filtrosItens)).ToList();

        foreach (var item in itensModel)
        {
            var imagemCommand = new GeraCaminhoImagemCommand()
            {
                CorCodigo = item.Versao,
                ModeloCodigo = item.Modelo
            };
            item.Imagem = await mediator.Send(imagemCommand, cancellationToken);

            pedidoModel.Itens.Add(item);
        }

        return pedidoModel;
    }
}
