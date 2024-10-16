using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.BrasilAbertoCEP;
using BlessWebPedidoSidi.Application.Cliente.Pesquisa;
using BlessWebPedidoSidi.Application.ReceitaFederal;
using BlessWebPedidoSidi.Application.ViaCEP;
using BlessWebPedidoSidi.Api.Controllers;
using BlessWebPedidoSidi.Application.Cliente;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BlessWebPedidoSidi.Api.Models.Clientes;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[Route("v{version:apiVersion}/cliente")]
[Authorize]
[ApiController]
public class ClienteController(IMediator mediator) : DefaultController
{

    /// <summary>
    /// Retorna os cliente por CNPJ ou Nome
    /// </summary>
    /// <response code="200">Clientes disponivel</response> 
    [ProducesResponseType(typeof(IList<ClienteModel>), StatusCodes.Status200OK)]
    [HttpGet("pesquisa")]
    public async Task<IActionResult> PesquisaAsync([FromQuery] ClientePesquisaRequest request)
    {
        var query = new PesquisaClienteQuery()
        {
            CnpjOuNome = request.CnpjOuNome ?? "",
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            CnpjCpf = "",
            ApenasClientesDoRepresentante = true,
            Pagina = request.Pagina,
            RegistrosPorPagina = request.RegistrosPorPagina
        };

        var clientes = await mediator.Send(query);
        return Ok(clientes);
    }

    /// <summary>
    /// Consulta os dados do cliente na Receita Federal
    /// </summary>
    /// <response code="200">Dados do cliente </response> 
    [ProducesResponseType(typeof(IList<ClienteModel>), StatusCodes.Status200OK)]
    [HttpGet("{cnpj}/consulta-receita")]
    public async Task<IActionResult> ConsultaDadosReceitaAsync([FromRoute] string cnpj)
    {
        var query = new ConsultaDadosReceitaFederalQuery(cnpj);
        var response = await mediator.Send(query);
        return Ok(response);
    }


    /// <summary>
    /// Consulta os dados do cliente cep
    /// </summary>
    /// <response code="200">Dados do cliente cep </response> 
    [ProducesResponseType(typeof(IList<ClienteModel>), StatusCodes.Status200OK)]
    [HttpGet("{Cep}/consulta-cep-old")]
    public async Task<IActionResult> ConsultaDadosBrasilAbertoCEPAsync([FromRoute] string Cep)
    {
        var query = new ConsultaDadosBrasilAbertoCEPQuery(Cep);
        var response = await mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Consulta os dados do cliente cep na api da ViaCep
    /// </summary>
    /// <response code="200">Dados do cliente CEP</response> 
    [ProducesResponseType(typeof(IList<ClienteModel>), StatusCodes.Status200OK)]
    [HttpGet("consulta-cep")]
    public async Task<IActionResult> ConsultaDadosViaCepAsync([FromQuery] string cep)
    {
        var query = new RetornaDadosViaCEPQuery() { Cep = cep };
        var response = await mediator.Send(query);
        return Ok(response);
    }
}
