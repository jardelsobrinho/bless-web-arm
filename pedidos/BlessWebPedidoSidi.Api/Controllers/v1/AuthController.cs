using Asp.Versioning;
using BlessWebPedidoSidi.Api.Models.Auths;
using BlessWebPedidoSidi.Application.Auth;
using BlessWebPedidoSidi.Application.Auth.AtualizaToken;
using BlessWebPedidoSidi.Application.Auth.GeraToken;
using BlessWebPedidoSidi.Application.Auth.RealizaLogin;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Domain.Usuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/auth")]
public class AuthController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Realiza o login do usuário
    /// </summary>
    /// <response code="200">Retorna o token</response> 
    [ProducesResponseType(typeof(DadosTokenGeradoModel), StatusCodes.Status200OK)]
    [HttpPost("login")]
    public async Task<IActionResult> RealizaLoginAsync([FromBody] GeraLoginRequest request)
    {
        var command = new RealizaLoginCommand()
        {
            ApiUrl = request.ApiUrl,
            Password = new PasswordValueObject(request.Password),
            Usuario = request.Usuario,
            RepresentanteCnpj = request.RepresentanteCnpj
        };

        var dadosTokenGerado = await mediator.Send(command);
        return Ok(dadosTokenGerado);
    }

    /// <summary>
    /// Atualiza token
    /// </summary>
    /// <response code="200">Token token atualizado</response> 
    [HttpPost("atualiza-token")]
    [ProducesResponseType(typeof(DadosTokenModel), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> AtualizaTokenAsync(AtualizaTokenRequest request)
    {
        var command = new AtualizaTokenCommand()
        {
            ApiUrl = "",
            TokenRefresh = request.TokenRefresh,
        };

        var dadosToken = await mediator.Send(command);
        return Ok(dadosToken);
    }

    [HttpGet]
    public IActionResult TesteAuth()
    {
        return Ok("ACESSO AUTORIZADO");
    }

    /// <summary>
    /// Retorna versão da api
    /// </summary>
    /// <response code="200">Versão da api</response> 
    [HttpGet("version")]
    public IActionResult RetornaVersion(IOptions<AppSettings> appSettings)
    {
        return Ok(new
        {
            version = appSettings.Value.Version,
            data = appSettings.Value.Data
        });
    }
}
