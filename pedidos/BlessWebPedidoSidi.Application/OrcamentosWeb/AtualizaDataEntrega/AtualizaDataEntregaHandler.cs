using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using MediatR;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaDataEntrega;

public class AtualizaDataEntregaHandler(IMediator mediator) : IRequestHandler<AtualizaDataEntregaCommand, AtualizaDataEntregaModel>
{

    public async Task<AtualizaDataEntregaModel> Handle(AtualizaDataEntregaCommand command, CancellationToken cancellationToken)
    {
        var pesquisaControleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(pesquisaControleSistemaPedidoQuery, cancellationToken);

        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var dataEntregaOrcamento = controleSistemaPedido.PreenchePrevisaoEntrega == "A"
                ? RetornaDataQuinzena(command.DataEntrega, controleSistemaPedido)
                : command.DataEntrega;

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        orcamento.DataEntrega = dataEntregaOrcamento;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return new AtualizaDataEntregaModel(dataEntregaOrcamento);
    }

    private static DateTime RetornaDataQuinzena(DateTime data, ControleSistemaPedidoModel preferencias)
    {
        int ano = data.Year;
        int mes = data.Month;
        int dia = data.Day;

        if (dia < preferencias.DiaPrimeiraQuinzena)
        {
            return new DateTime(ano, mes, preferencias.DiaPrimeiraQuinzena);
        }

        if (dia > preferencias.DiaPrimeiraQuinzena)
        {
            if (mes == 2)
            {
                int ultimoDiaFevereiro = DateTime.IsLeapYear(ano) ? 29 : 28;
                return new DateTime(ano, mes, ultimoDiaFevereiro);
            }

            return new DateTime(ano, mes, preferencias.DiaSegundaQuinzena);
        }

        return data;
    }

}

public record AtualizaDataEntregaCommand() : IRequest<AtualizaDataEntregaModel>
{
    public required string Uuid { get; init; }
    public required DateTime DataEntrega { get; init; }
    public required string RepresentanteCnpj { get; init; }
    public required string EmpresaCnpj { get; init; }
    public required int UsuarioCodigo { get; init; }
}
