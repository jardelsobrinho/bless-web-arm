using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Api.Models.Pedidos;
using BlessWebPedidoSidi.Application.Pedidos.PesquisaPedidos;
using BlessWebPedidoSidi.Application.Pedidos.RetornaDadosPedido;
using BlessWebPedidoSidi.Application.Pedidos.RetornaGradeItem;
using BlessWebPedidoSidi.Application.Pedidos.RetornaNotasFiscais;
using BlessWebPedidoSidi.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/pedido")]
[Authorize]
[ApiController]
public class PedidoController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Retorna pedidos
    /// </summary>
    /// <response code="200">Lista de pedidos</response> 
    [ProducesResponseType(typeof(IPaginacao<PesquisaPedidosModel>), StatusCodes.Status200OK)]
    [HttpGet("pesquisa")]
    public async Task<IActionResult> PesquisaPedidosAsync([FromQuery] PesquisaPedidoRequest request)
    {
        var query = new PesquisaPedidosQuery()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            NumeroOuNomeCliente = request.NumeroOuNomeCliente ?? "",
            DataEmissao = request.DataEmissao,
            Pagina = request.Pagina,
            RegistrosPorPagina = request.RegistrosPorPagina
        };

        var listaPedidos = await mediator.Send(query);
        return Ok(listaPedidos);
    }

    /// <summary>
    /// Retorna inforamações do pedido
    /// </summary>
    /// <response code="200">Inforamações do pedido</response> 
    [ProducesResponseType(typeof(RetornaDadosPedidoModel), StatusCodes.Status200OK)]
    [HttpGet("{pedido}")]
    public async Task<IActionResult> RetornaDadosPedidoAsync([FromRoute] int pedido)
    {
        var query = new RetornaDadosPedidoQuery()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Pedido = pedido
        };

        var pedidoModel = await mediator.Send(query);
        return Ok(pedidoModel);
    }

    /// <summary>
    /// Retorna a grade o item do pedido
    /// </summary>
    /// <response code="200">Grade do pedido</response> 
    [ProducesResponseType(typeof(RetornaDadosPedidoModel), StatusCodes.Status200OK)]
    [HttpGet("grade-item")]
    public async Task<IActionResult> RetornaGradeItem([FromQuery] RetornaGradeItemRequest request)
    {
        var query = new RetornaGradeItemQuery()
        {
            Pedido = request.Pedido,
            Sequencia = request.Sequencia,
        };

        var gradeItem = await mediator.Send(query);
        return Ok(gradeItem);
    }

    /// <summary>
    /// Retorna notas fiscais do pedido
    /// </summary>
    /// <response code="200">Lista de notas fiscais</response> 
    [ProducesResponseType(typeof(RetornaDadosPedidoModel), StatusCodes.Status200OK)]
    [HttpGet("notas-fiscais")]
    public async Task<IActionResult> RetornaNotasFiscais([FromQuery] RetornaNotasFiscaisRequest request)
    {
        var query = new RetornaNotasFiscaisQuery()
        {
            Pedido = request.Pedido,
            Sequencia = request.Sequencia,
        };

        var notasFiscais = await mediator.Send(query);
        return Ok(notasFiscais);
    }
}
