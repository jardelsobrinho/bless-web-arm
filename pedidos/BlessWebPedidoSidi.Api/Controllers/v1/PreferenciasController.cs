using Asp.Versioning;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Api.Models.Preferencias;
using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using BlessWebPedidoSidi.Application.ControleSistemaPedidoSidi;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/preferencias")]
[Authorize]
[ApiController]
public class PreferenciasController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Retorna preferencias
    /// </summary>
    /// <response code="200">Dados das preferencia do sistema</response> 
    [ProducesResponseType(typeof(PreferenciasResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> RetornaPreferenciasAsync()
    {
        var controleSistema = await mediator.Send(new ControleSistemaQuery());
        var controleSistemaPedidoSidi = await mediator.Send(new ControleSistemaPedidoSidiQuery());
        var controleSistemaPedido = await mediator.Send(new ControleSistemaPedidoQuery());

        var preferencias = new PreferenciasResponse()
        {
            UsarPrazoMedio = controleSistema.UsarPrazoMedio,
            GradeEspecialPedido = controleSistema.GradeEspecialPedido,
            FabricaPadrao = controleSistema.FabricaPadrao,
            EmpresaCnpj = controleSistema.EmpresaCnpj,
            ExibirPeca = controleSistema.ExibirPeca,
            SelecionaTabelaPreco = controleSistemaPedido.SelecionaTabelaPreco,
            FiltrarPrazoMedioCondicoes = controleSistemaPedido.FiltrarPrazoMedioCondicoes,
            PastaAwsS3 = controleSistemaPedido.PastaAwsS3,
            ValidaEstoqueAcabadoSibMobile = controleSistemaPedido.ValidaEstoqueAcabadoSibMobile,
            ValidaEstoqueDisponivel = controleSistemaPedido.ValidaEstoqueDisponivel,
            DiaPrimeiraQuinzena = controleSistemaPedido.DiaPrimeiraQuinzena,
            DiaSegundaQuinzena = controleSistemaPedido.DiaSegundaQuinzena,
            PreenchePrevisaoEntrega = controleSistemaPedido.PreenchePrevisaoEntrega,
            ExibirPrsMultiploGradeWeb = controleSistemaPedido.ExibirPrsMultiploGradeWeb,
            MarcaPedido = controleSistemaPedido.MarcaPedido,
            PermitirCoresTabelaPreco = controleSistemaPedido.PermitirCoresTabelaPreco,
            PreencherPrevisaoEntrega = controleSistemaPedidoSidi.PreencherPrevisaoEntrega
        };

        return Ok(preferencias);
    }
}
