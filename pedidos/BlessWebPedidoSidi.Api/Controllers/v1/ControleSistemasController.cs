using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/controleSistema")]
[Authorize]
[ApiController]
public class ControleSistemasController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Retorna preferencias
    /// </summary>
    /// <response code="200">Dados das preferencia do sistema</response> 
    [ProducesResponseType(typeof(ControleSistemaModel), StatusCodes.Status200OK)]
    [HttpGet("preferencias")]
    public async Task<IActionResult> RetornaPreferenciasAsync()
    {
        var controleSistema = await mediator.Send(new ControleSistemaQuery());
        return Ok(controleSistema);
    }

    /// <summary>
    /// Retorna preferencias pedido
    /// </summary>
    /// <response code="200">Dados das preferencia do pedido</response> 
    [ProducesResponseType(typeof(ControleSistemaPedidoModel), StatusCodes.Status200OK)]
    [HttpGet("preferenciasPedido")]
    public async Task<IActionResult> RetornaPreferenciasPedidoAsync()
    {
        var query = new ControleSistemaPedidoQuery()
        {
        };

        var preferenciaPedidoModel = await mediator.Send(query);
        return Ok(preferenciaPedidoModel);
    }
}
