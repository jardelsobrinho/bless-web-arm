using BlessWebPedidoSidi.Api.Authorization;
using BlessWebPedidoSidi.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlessWebPedidoSidi.Api.Controllers;

[ApiController]
public class DefaultController : ControllerBase
{
    protected DadosTokenModel DadosToken => (DadosTokenModel)HttpContext.Items[Contexts.DadosToken]!;
}
