using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BlessWebPedidoSidi.Application.ViaCEP;

public class RetornaDadosViaCEPHandler(IUnitOfWork unitOfWork) : IRequestHandler<RetornaDadosViaCEPQuery, RetornaDadosViaCEPModel>
{
    public async Task<RetornaDadosViaCEPModel> Handle(RetornaDadosViaCEPQuery query, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var client = new HttpClient(clientHandler)
        {
            BaseAddress = new Uri($"https://viacep.com.br/ws/")
        };

        var cep = Regex.Replace(query.Cep, @"\s+", "");
        var response = await client.GetAsync($"{cep}/json/", cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new BadHttpRequestException("RDVC01 - Falha na consulta do VIACEP");

        var dados = await response.Content.ReadFromJsonAsync<RetornaDadosViaCEPModel>(cancellationToken: cancellationToken);
        if (dados!.Erro)
            throw new BadHttpRequestException("RDVC02 - Falha na consulta do VIACEP. CEP Inválido!");

        var cidadeCodigo = await unitOfWork.CidadeRepository.RetornaCodigoClienteAsync(dados!.Ibge, dados!.Localidade);
        dados = dados with { CidadeCodigo = cidadeCodigo };
        return dados;
    }

}

public record RetornaDadosViaCEPQuery : IRequest<RetornaDadosViaCEPModel>
{
    public required string Cep { get; init; }
}
