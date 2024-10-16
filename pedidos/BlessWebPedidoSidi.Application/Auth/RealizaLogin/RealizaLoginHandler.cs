using BlessWebPedidoSidi.Application.Auth.GeraToken;
using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Domain.Shared;
using BlessWebPedidoSidi.Domain.Usuario;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.Auth.RealizaLogin;

public class RealizaLoginHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<RealizaLoginCommand, DadosLoginModel>
{

    public async Task<DadosLoginModel> Handle(RealizaLoginCommand request, CancellationToken cancellationToken)
    {
        var consultaUsuario = await unitOfWork.UsuarioRepository.PesquisaAsync(x => x.Usuario.Equals(request.Usuario)
        && x.CnpjRepresentante == request.RepresentanteCnpj);

        if (consultaUsuario.Count == 0)
            throw new BadHttpRequestException("RLH01 - Usuario/Password/Cnpj inválido");

        var usuario = consultaUsuario.First();
        var novoPassword = request.Password;
        if (novoPassword.Value != usuario.Password.Value)
            throw new BadHttpRequestException("RLH02 - Usuario/Password/Cnpj inválido");

        var controleSistemaQuery = new ControleSistemaQuery();
        var dadosEmpresa = await mediator.Send(controleSistemaQuery);
        var geraTokenCommand = new GeraTokenCommand()
        {
            ApiUrl = request.ApiUrl,
            RepresentanteCnpj = request.RepresentanteCnpj,
            EmpresaCnpj = dadosEmpresa.EmpresaCnpj!,
            UsuarioNome = request.Usuario,
            UsuarioCodigo = usuario.Codigo,
            TokenRefresh = false
        };

        var dadosToken = await mediator.Send(geraTokenCommand, cancellationToken);

        geraTokenCommand.TokenRefresh = true;
        var dadosTokenRefresh = await mediator.Send(geraTokenCommand, cancellationToken);

        var dadosLogin = new DadosLoginModel()
        {
            Token = dadosToken.Token,
            TokenDataValidade = dadosToken.TokenDataValidade,
            TokenRefresh = dadosTokenRefresh.Token
        };

        return dadosLogin;
    }
}

public record RealizaLoginCommand : IRequest<DadosLoginModel>
{
    public required string Usuario { get; set; }
    public required PasswordValueObject Password { get; set; }
    public required string ApiUrl { get; set; }
    public required string RepresentanteCnpj { get; set; }
}