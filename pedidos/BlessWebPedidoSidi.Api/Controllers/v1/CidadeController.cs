using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.Cidades;
using BlessWebPedidoSidi.Api.Controllers;
using BlessWebPedidoSidi.Application.Ufs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/cidade")]
[Authorize]
[ApiController]
public class CidadeController(IMediator mediator) : DefaultController
{

    /// <summary>
    /// Retorna lista de cidades
    /// </summary>
    /// <response code="200">cidades</response> 
    [ProducesResponseType(typeof(IList<UfModel>), StatusCodes.Status200OK)]
    [HttpGet()]
    public async Task<IActionResult> RetornaCidadesAsync([FromQuery] string ufSigla, string? cidadeNome)
    {
        var query = new RetornaCidadesQuery()
        {
            UfSigla = ufSigla,
            CidadeNome = cidadeNome ?? ""
        };
        var listaCidades = await mediator.Send(query);
        return Ok(listaCidades);
    }

}
