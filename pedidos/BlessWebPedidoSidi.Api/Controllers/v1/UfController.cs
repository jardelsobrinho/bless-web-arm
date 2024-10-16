using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.Ufs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/uf")]
[Authorize]
[ApiController]
public class UfController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Retorna lista de UFs
    /// </summary>
    /// <response code="200">UFs</response> 
    [ProducesResponseType(typeof(IList<UfModel>), StatusCodes.Status200OK)]
    [HttpGet()]
    public async Task<IActionResult> RetornaUfsAsync()
    {
        var query = new RetornaUfsQuery();
        var listaUfs = await mediator.Send(query);
        return Ok(listaUfs);
    }
}
