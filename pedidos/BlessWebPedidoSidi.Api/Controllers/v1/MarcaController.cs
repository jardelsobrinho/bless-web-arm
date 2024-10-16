using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.Marcas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/marca")]
[Authorize]
[ApiController]
public class MarcaController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Retorna lista de Marcas
    /// </summary>
    /// <response code="200">Marca</response> 
    [ProducesResponseType(typeof(IList<MarcaModel>), StatusCodes.Status200OK)]
    [HttpGet()]
    public async Task<IActionResult> RetornaMarcasAsync()
    {
        var query = new RetornaMarcaQuery() { UsuarioCodigo = DadosToken.UsuarioCodigo };
        var listaMarcas = await mediator.Send(query);
        return Ok(listaMarcas);
    }
}
