using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.ClientesWeb.GravaClienteWeb;
using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using BlessWebPedidoSidi.Api.Models.ClientesWeb;
using BlessWebPedidoSidi.Application.ClientesWeb.RetornaClienteWebPorId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/cliente-web")]
[Authorize]
[ApiController]
public class ClienteWebController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Gravar um cliente que só sera adicionado quando o pedido for gravado.
    /// </summary>
    /// <remarks>
    /// Exemplo:
    ///
    ///     POST /v1/cliente-web
    ///     {
    ///        "tipoInscricaoEstadual": ["Contribuinte", "Isento", "NaoContribuinte"],
    ///        "enderecos": [
    ///          {
    ///             "tipo": ["Principal", "Entrega", "Contribuinte"]
    ///          }
    ///        ],
    ///        "enderecosExcluidos": ["Principal", "Entrega", "Contribuinte"]
    ///     }
    ///
    /// </remarks>/// 
    /// <response code="200">Id do cliente gravado</response> 
    [ProducesResponseType(typeof(ClienteWebIdResponse), StatusCodes.Status200OK)]
    [HttpPost()]
    public async Task<IActionResult> GravaClienteAsync([FromBody] ClienteWebRequest request)
    {
        var query = new GravaClienteWebCommand()
        {
            Id = request.Id,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            RazaoSocial = request.RazaoSocial,
            CelularDDD = request.CelularDDD,
            CelularNumero = request.CelularNumero,
            CnpjCpf = request.CnpjCpf,
            ContatoEmail = request.ContatoEmail,
            ContatoNome = request.ContatoNome,
            InscricaoEstadual = request.InscricaoEstadual,
            NomeFantasia = request.NomeFantasia,
            TelefoneDDD = request.TelefoneDDD,
            TelefoneNumero = request.TelefoneNumero,
            TipoInscricaoEstadual = request.TipoInscricaoEstadual != null && request.TipoInscricaoEstadual != "" ?
                (ETipoInscricaoEstadual)Enum.Parse(typeof(ETipoInscricaoEstadual), request.TipoInscricaoEstadual!) : null,
            EnderecoCobrancaIgualPrincipal = request.EnderecoCobrancaIgualPrincipal ? "S" : "N",
            EnderecoEntregaIgualPrincipal = request.EnderecoEntregaIgualPrincipal ? "S" : "N",
            Enderecos = request.Enderecos.Select(x => new GravaClienteWebEnderecoCommand()
            {
                Bairro = x.Bairro,
                Cep = x.Cep,
                CidadeId = x.CidadeId,
                Complemento = x.Complemento,
                Numero = x.Numero,
                Rua = x.Rua,
                Tipo = (ETipoEndereco)Enum.Parse(typeof(ETipoEndereco), x.Tipo)
            }).ToList(),
            EnderecosIdsExcluidos = request.EnderecosIdsExcluidos ?? new List<int>(),
            FretePorConta = request.FretePorConta,
            UsuarioCodigo = DadosToken.UsuarioCodigo

        };
        var clienteId = await mediator.Send(query);
        return Ok(new ClienteWebIdResponse() { ClienteId = clienteId });
    }

    /// <summary>
    /// Retorna um cliente pelo id
    /// </summary>
    /// <response code="200">Dados do cliente</response> 
    [ProducesResponseType(typeof(ClienteWebModel), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<IActionResult> RetornaPorIdAsync([FromRoute] int id)
    {
        var query = new RetornaClienteWebPorIdQuery()
        {
            ClienteId = id,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj
        };

        var clienteModel = await mediator.Send(query);
        return Ok(clienteModel);
    }

    /*
    /// <summary>
    /// Pesquisa clientes
    /// </summary>
    /// <response code="200">Lista de clientes</response> 
    /*[ProducesResponseType(typeof(IList<ClientePesquisaModel>), StatusCodes.Status200OK)]
    [HttpGet("pesquisa")]
    public async Task<IActionResult> PesquisaAsync([FromQuery] string? cnpjOuNome)
    {
        var query = new ClientePesquisaQuery()
        {
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            CnpjOuNome = cnpjOuNome ?? ""
        };

        var listaClientes = await _mediator.Send(query);
        return Ok(listaClientes);
    }*/
}
