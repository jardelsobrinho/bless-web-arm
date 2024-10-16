using BlessWebPedidoSidi.Domain.Shared;
using BlessWebPedidoSidi.Application.Auth;
using BlessWebPedidoSidi.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BlessWebPedidoSidi.Application.Auth.ValidaToken;

public class ValidaTokenHandler(IOptions<AppSettings> appSettings, IUnitOfWork unitOfWork, ICriptografia criptografia) : IRequestHandler<ValidaTokenCommand, DadosTokenModel>
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<DadosTokenModel> Handle(ValidaTokenCommand command, CancellationToken cancellationToken)
    {
        if (command.Token.IsNullOrEmpty())
            throw new BadHttpRequestException("VTH01 - Token não informado");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
        try
        {
            tokenHandler.ValidateToken(command.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var usuarioNome = criptografia.Decrypt(jwtToken.Claims.First(x => x.Type == ClaimsConfig.ClaimUser).Value);
            var usuarioCodigo = criptografia.Decrypt(jwtToken.Claims.First(x => x.Type == ClaimsConfig.ClaimUserId).Value);
            var represenanteCnpj = criptografia.Decrypt(jwtToken.Claims.First(x => x.Type == ClaimsConfig.ClaimDoc).Value);
            var empresaCnpj = criptografia.Decrypt(jwtToken.Claims.First(x => x.Type == ClaimsConfig.ClaimDocClient).Value);
            var origem = jwtToken.Claims.First(x => x.Type == ClaimsConfig.ClaimOrigin).Value;
            var claimRefresh = criptografia.Decrypt(jwtToken.Claims.First(x => x.Type == ClaimsConfig.ClaimRefresh).Value);

            if (command.ValidaTokenRefresh && claimRefresh == "NAO")
            {
                return DadosTokenModel.CriaDadosTokenComErro("JWS02 - Token de refresh inválido");
            }

            if (!command.ValidaTokenRefresh && claimRefresh == "SIM")
            {
                return DadosTokenModel.CriaDadosTokenComErro("JWS04 - Token inválido");
            }

            var consultaEmpresa = await unitOfWork.ControleSistemaRepository.PesquisaAsync(x => x.Cnpj == empresaCnpj);
            if (consultaEmpresa.Count == 0)
                throw new BadHttpRequestException("VTH02 - Empresa inválida para esse token");

            return new DadosTokenModel()
            {
                UsuarioNome = usuarioNome,
                UsuarioCodigo = int.Parse(usuarioCodigo),
                EmpresaCnpj = empresaCnpj,
                RepresentanteCnpj = represenanteCnpj
            };
        }
        catch (Exception e)
        {
            return DadosTokenModel.CriaDadosTokenComErro(e.Message);
        }
    }
}

public record ValidaTokenCommand : IRequest<DadosTokenModel>
{
    public required string Token { get; set; }
    public required bool ValidaTokenRefresh { get; set; }
}