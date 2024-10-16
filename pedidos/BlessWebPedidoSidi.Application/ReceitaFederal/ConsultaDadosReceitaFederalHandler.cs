using BlessWebPedidoSidi.Application.Cidades;
using BlessWebPedidoSidi.Application.ReceitaFederal.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace BlessWebPedidoSidi.Application.ReceitaFederal;

public class ConsultaDadosReceitaFederalHandler(IMediator mediator) : IRequestHandler<ConsultaDadosReceitaFederalQuery, DadosRFModel>
{
    public async Task<DadosRFModel> Handle(ConsultaDadosReceitaFederalQuery query, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var client = new HttpClient(clientHandler)
        {
            BaseAddress = new Uri("https://receitaws.com.br")
        };
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await client.GetAsync($"/v1/cnpj/{query.Cnpj}");

        if (response.IsSuccessStatusCode)
        {
            var dados = await response.Content.ReadFromJsonAsync<DadosRFModel>(cancellationToken: cancellationToken) ??
                        throw new BadHttpRequestException("CDRFH01 - Nenhuma informação retornada");

            if (dados.Nome == "")
                throw new BadHttpRequestException("CDRFH03 - Não foi encontrado nenhuma informação com esse CNPJ");

            var retornaCodigoCidade = new RetornaCodigoCidadeQuery()
            {
                CidadeCep = dados.Cep,
                CidadeNome = dados.Municipio,
                UfSigla = dados.Uf
            };

            var codigoCidade = await mediator.Send(retornaCodigoCidade, cancellationToken);
            dados = dados with { MunicipioCodigo = codigoCidade };
            dados = dados with { Cep = Regex.Replace(dados.Cep, "[^a-zA-Z0-9/-]+", "", RegexOptions.Compiled) };

            return dados;

        }
        var erro = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
        throw new BadHttpRequestException($"CDRFH02 - Falha ao consulta a api do Sidi - " + erro!.Title);
    }
}

public record ConsultaDadosReceitaFederalQuery(string Cnpj) : IRequest<DadosRFModel>;

