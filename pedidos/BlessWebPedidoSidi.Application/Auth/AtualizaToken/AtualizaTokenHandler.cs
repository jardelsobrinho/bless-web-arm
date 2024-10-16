using BlessWebPedidoSidi.Application.Auth.RealizaLogin;
using BlessWebPedidoSidi.Application.Auth.ValidaToken;
using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.Auth.AtualizaToken;

public class AtualizaTokenHandler(IMediator mediator, IUnitOfWork unitOfWork) : IRequestHandler<AtualizaTokenCommand, DadosLoginModel>
{
    public async Task<DadosLoginModel> Handle(AtualizaTokenCommand request, CancellationToken cancellationToken)
    {
        var validaToken = new ValidaTokenCommand()
        {
            Token = request.TokenRefresh,
            ValidaTokenRefresh = true
        };
        var dadosToken = await mediator.Send(validaToken);
        if (dadosToken.Erro != "" && dadosToken.Erro != null)
            throw new BadHttpRequestException($"ATH02 - Token refresh invalido - {dadosToken.Erro}");

        var usuario = await unitOfWork.UsuarioRepository.CarregaDadosAsync(x => x.Codigo == dadosToken.UsuarioCodigo)
                ?? throw new BadHttpRequestException("ATH01 - Usuário não encontrado");

        var realizaLoginCommand = new RealizaLoginCommand()
        {
            ApiUrl = request.ApiUrl,
            Password = usuario.Password,
            Usuario = dadosToken.UsuarioNome,
            RepresentanteCnpj = dadosToken.RepresentanteCnpj
        };

        return await mediator.Send(realizaLoginCommand);
    }
}

public record AtualizaTokenCommand : IRequest<DadosLoginModel>
{
    public required string TokenRefresh { get; set; }
    public required string ApiUrl { get; set; }
}
