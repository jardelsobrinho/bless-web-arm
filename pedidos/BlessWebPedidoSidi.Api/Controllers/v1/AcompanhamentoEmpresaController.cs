using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Api.Models.AcompanhamentoEmpresa;
using BlessWebPedidoSidi.Api.Models.OrcamentoWeb;
using BlessWebPedidoSidi.Api.Controllers;
using BlessWebPedidoSidi.Application.AcompanhamentoEmpresa;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/acompanhamento")]
    [ApiController]
    public class AcompanhamentoEmpresaController(IMediator mediator) : DefaultController
    {

        /// <summary>
        /// Retornar informações do orçamento
        /// </summary>
        /// <response code="200">Retorna informações do orçamento</response> 
        [ProducesResponseType(typeof(DadosEmpresaModel), StatusCodes.Status200OK)]
        [HttpGet("dados-empresa")]
        public async Task<IActionResult> RetornaDadosEmpresaAsync([FromQuery] DateTime? DataInicial, DateTime? DataFinal)
        {
            var query = new RetornaDadosOrcamentoEmpresaQuery()
            {
                DataInicial = DataInicial ?? new DateTime(),
                DataFinal = DataFinal ?? new DateTime()
            };

            var dadosOrcamento = await mediator.Send(query);
            return Ok(dadosOrcamento);
        }
    }
}
