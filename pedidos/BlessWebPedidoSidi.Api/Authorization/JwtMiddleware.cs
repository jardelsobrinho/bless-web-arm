using BlessWebPedidoSidi.Application.Auth.ValidaToken;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace BlessWebPedidoSidi.Api.Authorization;

public class JwtMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, IMediator _mediator)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token == null)
        {
            context.Items[Contexts.ErroToken] = "JM01 - Token usuário não informado";
        }
        else
        {
            var validaTokenCommand = new ValidaTokenCommand()
            {
                Token = token,
                ValidaTokenRefresh = false
            };
            var dadosTokenModel = await _mediator.Send(validaTokenCommand);
            if (dadosTokenModel.Erro.IsNullOrEmpty())
            {
                context.Items[Contexts.DadosToken] = dadosTokenModel;
            }
            else
            {
                context.Items[Contexts.ErroToken] = dadosTokenModel.Erro;
            }
        }

        await _next(context);
        return;
    }
}
