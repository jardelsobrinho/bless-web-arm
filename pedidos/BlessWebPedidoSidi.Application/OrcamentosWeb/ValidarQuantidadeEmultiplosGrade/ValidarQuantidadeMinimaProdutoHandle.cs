using BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;
using BlessWebPedidoSidi.Application.OrcamentosWeb.ValidarQuantidadeEmultiplosGrade.Models;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.ValidarQuantidadeEmultiplosGrade;
public class ValidarQuantidadeMinimaProdutoHandle(IDbConnection conexao) : IRequestHandler<ValidarQuantidadeMinimaProdutoQuery, ValidarQuantidadeMinimaProdutoModel>
{
    public async Task<ValidarQuantidadeMinimaProdutoModel> Handle(ValidarQuantidadeMinimaProdutoQuery query, CancellationToken cancellationToken)
    {
        var sqlModeloPreferencia = $"SELECT M.VALIDA_GRADE_PEDIDO ValidarGradePedido, M.QUANTIDADE_MINIMA QuantidadeMinima FROM MODELOS M WHERE MODELO = {query.ModeloCodigo} ";
        var preferenciaModelo = (await conexao.QueryAsync<PreferenciaModeloModel>(sqlModeloPreferencia)).FirstOrDefault() ?? throw new BadHttpRequestException("Preferência de modelo não encontrada.");
        var mensagem = new ValidarQuantidadeMinimaProdutoModel();

        if (preferenciaModelo.ValidarGradePedido == "M")
        {
            if (query.QuantidadeInformada < preferenciaModelo.QuantidadeMinima)
            {
                throw new BadHttpRequestException($"A quantidade deve corresponder a {preferenciaModelo.QuantidadeMinima} pares, conforme predefinido no cadastro deste modelo.");
            }

            if (query.QuantidadeInformada % preferenciaModelo.QuantidadeMinima != 0)
            {
                mensagem.MensagemRetorno = $"{query.QuantidadeInformada} não é múltiplo de {preferenciaModelo.QuantidadeMinima}";
            }
        }
        return mensagem;
    }
}

public record ValidarQuantidadeMinimaProdutoQuery : IRequest<ValidarQuantidadeMinimaProdutoModel>
{
    public int ModeloCodigo { get; set; }
    public int QuantidadeInformada { get; set; }
}