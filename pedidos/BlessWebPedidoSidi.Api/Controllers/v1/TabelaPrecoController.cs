using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Application.CondicaoPagamento.PesquisaCondicaoPagamento;
using BlessWebPedidoSidi.Application.TabelaPreco.PesquisaTabelaPreco;
using BlessWebPedidoSidi.Api.Controllers;
using BlessWebPedidoSidi.Application.CondicaoPagamento;
using BlessWebPedidoSidi.Application.TabelaPreco;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[Route("v{version:apiVersion}/tabela-preco")]
[Authorize]
[ApiController]
public class TabelaPrecoController(IMediator mediator) : DefaultController
{

    /// <summary>
    /// Retorna as tabelas de preço por nome
    /// </summary>
    /// <response code="200">Tabelas de preço disponíveis</response> 
    [ProducesResponseType(typeof(IList<TabelaPrecoModel>), StatusCodes.Status200OK)]
    [HttpGet("pesquisa")]
    public async Task<IActionResult> PesquisaAsync([FromQuery] string? nome)
    {
        var query = new PesquisaTabelaPrecoQuery()
        {
            TabelaPrecoCodigo = 0,
            TabelaPrecoNome = nome ?? "",
            RepresentanteCnpj = DadosToken.RepresentanteCnpj
        };

        var tabelasDePreco = await mediator.Send(query);
        return Ok(tabelasDePreco);
    }

    /// <summary>
    /// Retorna as condições de pagamento por tabela de preço
    /// </summary>
    /// <response code="200">Condições de pagamento disponíveis</response> 
    [ProducesResponseType(typeof(IList<CondicaoPagamentoModel>), StatusCodes.Status200OK)]
    [HttpGet("{codigo}/condicao-pagamento/pesquisa")]
    public async Task<IActionResult> PesquisaCondicaoPagamentoAsync([FromRoute] int codigo, [FromQuery] string? descricao)
    {
        var query = new PesquisaCondicaoPagamentoQuery()
        {
            TabelaPrecoCodigo = codigo,
            CondicaoPagamentoDescricao = descricao ?? "",
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo
        };

        var condicoesDePagamento = await mediator.Send(query);

        return Ok(condicoesDePagamento);
    }
}
