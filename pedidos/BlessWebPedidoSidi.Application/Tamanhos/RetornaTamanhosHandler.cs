using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.Shared;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace BlessWebPedidoSidi.Application.Tamanhos;

public class RetornaTamanhosHandler(IDbConnection conexao, IMediator mediator) : IRequestHandler<RetornaTamanhosQuery, IList<RetornaTamanhoModel>>
{
    public async Task<IList<RetornaTamanhoModel>> Handle(RetornaTamanhosQuery request, CancellationToken cancellationToken)
    {
        var controleSistemaQuery = new ControleSistemaQuery();
        var controleSistema = await mediator.Send(controleSistemaQuery, cancellationToken);

        var campoDescricao = "LC.TAMANHO DESCRICAO";
        if (controleSistema.GradeEspecialPedido == "S")
            campoDescricao = "IIF(LC.GRADE_ESPECIAL IS NULL, LC.TAMANHO, LC.GRADE_ESPECIAL) DESCRICAO";

        var sqlTamanhos = new StringBuilder($"SELECT LC.TAMANHO CODIGO, {campoDescricao}, COALESCE(G.ORDEM, 0) ORDEM");
        sqlTamanhos.AppendSql("FROM LINHA_COORDENACAO LC");
        sqlTamanhos.AppendSql("LEFT JOIN GRADE_ESPECIAL_ORDENADA G ON G.TAMANHO = LC.TAMANHO");

        switch (request.TipoCodigo)
        {
            case TipoCodigoRetornaTamanho.Referencia:
                sqlTamanhos.AppendSql($"WHERE LC.CODIGO_LINHA = {request.Codigo}");
                break;

            case TipoCodigoRetornaTamanho.Modelo:
                sqlTamanhos.AppendSql("INNER JOIN MODELOS M ON M.FK_LINHA = LC.CODIGO_LINHA");
                sqlTamanhos.AppendSql($"WHERE M.MODELO = {request.Codigo}");
                break;
        }

        sqlTamanhos.AppendSql("ORDER BY G.ORDEM, LC.TAMANHO");

        return (await conexao.QueryAsync<RetornaTamanhoModel>(sqlTamanhos.ToString())).ToList();
    }
}
public enum TipoCodigoRetornaTamanho
{
    Referencia,
    Modelo
}
public record RetornaTamanhosQuery : IRequest<IList<RetornaTamanhoModel>>
{
    public TipoCodigoRetornaTamanho TipoCodigo { get; init; }
    public int Codigo { get; init; }
}
