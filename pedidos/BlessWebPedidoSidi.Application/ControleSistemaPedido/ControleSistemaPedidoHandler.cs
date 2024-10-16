using Dapper;
using MediatR;
using System.Data;

namespace BlessWebPedidoSidi.Application.ControleSistemaPedido;

public class ControleSistemaPedidoHandler(IDbConnection conexao) : IRequestHandler<ControleSistemaPedidoQuery, ControleSistemaPedidoModel>
{
    public async Task<ControleSistemaPedidoModel> Handle(ControleSistemaPedidoQuery request, CancellationToken cancellationToken)
    {
        var sql = @"SELECT SELECIONAR_TABELA_PRECO SelecionaTabelaPreco,
                    COALESCE(FILTRAR_PRAZO_MEDIO_CONDICOES, 'N') AS FiltrarPrazoMedioCondicoes,
                    COALESCE(VALIDAR_ESTOQUE_ACABADO_SID_MOB, 'F') AS ValidaEstoqueAcabadoSibMobile,
                    COALESCE(VALIDAR_ESTOQUE_DISPONIVEL, 'F') AS ValidaEstoqueDisponivel,
                    PASTA_AWS_S3 PastaAwsS3,
                    DIA_PRIMEIRA_QUINZENA DiaPrimeiraQuinzena,
                    DIA_SEGUNDA_QUINZENA DiaSegundaQuinzena,
                    PREENCHE_PREVISAO_ENTREGA PreenchePrevisaoEntrega,
                    EXIBIR_PARES_MULT_GRADE_WEB ExibirPrsMultiploGradeWeb,
                    MARCA_PEDIDO MarcaPedido,
                    PERMITIR_CORES_TABELA_PRECO PermitirCoresTabelaPreco
                    FROM CONTROLE_SISTEMA_PEDIDO_SIDI, CONTROLE_SISTEMA_PEDIDO";

        return (await conexao.QueryAsync<ControleSistemaPedidoModel>(sql)).First();
    }
}

public record ControleSistemaPedidoQuery : IRequest<ControleSistemaPedidoModel>;