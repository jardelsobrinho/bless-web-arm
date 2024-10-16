using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Api.Models.Modelo;
using BlessWebPedidoSidi.Application.Marcas;
using BlessWebPedidoSidi.Application.Modelos.PesquisaModelos;
using BlessWebPedidoSidi.Application.Modelos.RetornaModelo;
using BlessWebPedidoSidi.Application.Modelos.RetornaModelo.Models;
using BlessWebPedidoSidi.Application.Palmilhas.RetornaPalmilhas;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Application.Solados.RetornaSolados;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[Route("v{version:apiVersion}/modelo")]
[Authorize]
[ApiController]
public class ModeloController(IMediator mediator) : DefaultController
{

    /// <summary>
    /// Retorna os modelos por nome ou código
    /// </summary>
    /// <response code="200">Modelos disponivel</response> 
    [ProducesResponseType(typeof(IPaginacao<ModeloPesquisaModel>), StatusCodes.Status200OK)]
    [HttpGet("pesquisa")]
    public async Task<IActionResult> PesquisaAsync([FromQuery] ModeloPesquisaRequest request)
    {
        var query = new PesquisaModeloQuery
        {
            TabelaPrecoCodigo = request.TabelaPrecoCodigo,
            CondicaoPagamentoCodigo = request.CondicaoPagamentoCodigo,
            ModeloCodigo = 0,
            ReferenciaOuDescricao = request.ReferenciaOuDescricao ?? "",
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            Pagina = request.Pagina,
            RegistrosPorPagina = request.RegistrosPorPagina
        };

        var dados = await mediator.Send(query);
        return Ok(dados);
    }

    /// <summary>
    /// Retorna o modelo completo com as cores, grades e estoques
    /// </summary>
    /// <response code="200">Modelos completo</response> 
    [ProducesResponseType(typeof(ModeloModel), StatusCodes.Status200OK)]
    [HttpGet("{codigo}")]
    public async Task<IActionResult> RetornaModeloAsync([FromRoute] int codigo, [FromQuery] ModeloRetornoRequest request)
    {
        var query = new RetornaModeloQuery()
        {
            CondicaoPagamentoCodigo = request.CondicaoPagamentoCodigo ?? 0,
            ModeloCodigo = codigo,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            TabelaPrecoCodigo = request.TabelaPrecoCodigo ?? 0,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            OrcamentoUuid = request.OrcamentoUuid ?? ""
        };

        var modelo = await mediator.Send(query);
        return Ok(modelo);
    }

    /// <summary>
    /// Retorna o modelo com a cor, grades e estoques
    /// </summary>
    /// <response code="200">Modelos completo</response> 
    [ProducesResponseType(typeof(ModeloModel), StatusCodes.Status200OK)]
    [HttpGet("{codigo}/cor/{codigoCor}")]
    public async Task<IActionResult> RetornaModeloCorAsync([FromRoute] int codigo, [FromQuery] ModeloRetornoRequest request, [FromRoute] int codigoCor)
    {
        var query = new RetornaModeloQuery()
        {
            CondicaoPagamentoCodigo = request.CondicaoPagamentoCodigo ?? 0,
            ModeloCodigo = codigo,
            CodigoCor = codigoCor,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            TabelaPrecoCodigo = request.TabelaPrecoCodigo ?? 0,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            OrcamentoUuid = request.OrcamentoUuid ?? ""
        };

        var modelo = await mediator.Send(query);
        return Ok(modelo);
    }

    /// <summary>
    /// Retorna uma lista com as marcas, solados e palmilhas que podem ser usadas no modelo
    /// </summary>
    /// <response code="200">Lista de marcas, solados e palmilhas</response> 
    [ProducesResponseType(typeof(ModeloDadosCadastroResponse), StatusCodes.Status200OK)]
    [HttpGet("dados-cadastro")]
    public async Task<IActionResult> RetornaDadosCadastroAsync()
    {
        var listaMarcaQuery = new RetornaMarcaQuery() { UsuarioCodigo = DadosToken.UsuarioCodigo };
        var listaMarcas = await mediator.Send(listaMarcaQuery);

        var listaSoladosQuery = new RetornaSoladosQuery() { UsuarioCodigo = DadosToken.UsuarioCodigo };
        var listaSolados = await mediator.Send(listaSoladosQuery);

        var listaPalmilhasQuery = new RetornaPalmilhasQuery() { UsuarioCodigo = DadosToken.UsuarioCodigo };
        var listaPalmilhas = await mediator.Send(listaPalmilhasQuery);

        var response = new ModeloDadosCadastroResponse()
        {
            Marcas = listaMarcas,
            Solados = listaSolados,
            Palmilhas = listaPalmilhas
        };

        return Ok(response);
    }
}
