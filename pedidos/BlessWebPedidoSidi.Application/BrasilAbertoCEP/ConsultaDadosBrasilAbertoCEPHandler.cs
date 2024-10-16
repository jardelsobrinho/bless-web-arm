using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using BlessWebPedidoSidi.Application.BrasilAbertoCEP.Models;

namespace BlessWebPedidoSidi.Application.BrasilAbertoCEP;

public class ConsultaDadosBrasilAbertoCEPHandler : IRequestHandler<ConsultaDadosBrasilAbertoCEPQuery, DadosResultCEPModel<DadosCEPModel>>
{
    public async Task<DadosResultCEPModel<DadosCEPModel>> Handle(ConsultaDadosBrasilAbertoCEPQuery query, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var client = new HttpClient(clientHandler)
        {
            BaseAddress = new Uri("https://api.brasilaberto.com")
        };

        client.DefaultRequestHeaders.Add("Authorization", "Bearer tQrxGO4lLkVXWjjnMrhOnnjR7eqgYIcttLjkJuxrIVjRXCWRbLqMwbfViZanisF");

        var response = await client.GetAsync($"/v1/zipcode/{query.cep}");

        if (response.IsSuccessStatusCode)
        {
            var dados = await response.Content.ReadFromJsonAsync<DadosResultCEPModel<DadosCEPModel>>(cancellationToken: cancellationToken) ??
                        throw new BadHttpRequestException("CDBAH01 - Nenhuma informação retornada");

            if (dados.Result.Zipcode == "")
                throw new BadHttpRequestException("CDBAH03 - Não foi encontrado nenhuma informação com esse CEP");

            return dados;
        }
        var erro = await response.Content.ReadFromJsonAsync<DadosResultCEPModel<DadosErroModel>>(cancellationToken: cancellationToken);
        throw new BadHttpRequestException($"CDBAH02 - Falha API Brasil Aberto - " + erro!.Result.Message);
    }
}

public record ConsultaDadosBrasilAbertoCEPQuery(string cep) : IRequest<DadosResultCEPModel<DadosCEPModel>>;