using BlessWebPedidoSidi.Application.Cliente.Pesquisa;
using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaCliente;

public class AtualizaClienteOrcamentoHandler(IMediator mediator) : IRequestHandler<AtualizaClienteOrcamentoCommand, Unit>
{
    public async Task<Unit> Handle(AtualizaClienteOrcamentoCommand command, CancellationToken cancellationToken)
    {
        var pesquisaClienteQuery = new PesquisaClienteQuery()
        {
            CnpjOuNome = "",
            RepresentanteCnpj = command.RepresentanteCnpj,
            CnpjCpf = command.ClienteCnpjCpf,
            EmpresaCnpj = command.EmpresaCnpj,
            UsuarioCodigo = command.UsuarioCodigo,
            ApenasClientesDoRepresentante = true,
            Pagina = 1,
            RegistrosPorPagina = 10
        };

        var clienteHandle = await mediator.Send(pesquisaClienteQuery, cancellationToken);
        if (clienteHandle.Registros.Count == 0)
            throw new BadHttpRequestException($"ACH01 - Cliente não encontrado com o Cnpj/Cpf {command.ClienteCnpjCpf}");

        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);

        var clienteEntity = clienteHandle.Registros.First();
        orcamento.ClienteCnpjCpf = Regex.Match(clienteEntity.CnpjCpf, @"\d+").Value;
        orcamento.ClienteNome = clienteEntity.RazaoSocial;
        orcamento.Frete = clienteEntity.FretePorConta;

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }
}

public record AtualizaClienteOrcamentoCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required string ClienteCnpjCpf { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
}