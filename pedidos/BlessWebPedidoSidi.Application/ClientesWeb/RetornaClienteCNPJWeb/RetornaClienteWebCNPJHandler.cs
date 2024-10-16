using BlessSidi.Application.ClientesWeb.RetornaClienteWebPorId;
using BlessSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessSidi.Application.ClientesWeb.RetornaClienteWeb;

public class RetornaClienteWebCNPJHandler(IUnitOfWork unitOfWork) : IRequestHandler<RetornaClienteWebCNPJQuery, ClienteWebModel?>
{
    public async Task<ClienteWebModel?> Handle(RetornaClienteWebCNPJQuery request, CancellationToken cancellationToken)
    {
        var clienteEntity = (await unitOfWork.ClienteWebRepository.PesquisaAsync(
        x => x.CnpjCpf == request.CnpjCpf)).SingleOrDefault() ?? null;

        if(clienteEntity != null)
        {
            var clienteModel = new ClienteWebModel()
            {
                CelularDDD = clienteEntity.CelularDDD,
                CelularNumero = clienteEntity.CelularNumero,
                CnpjCpf = clienteEntity.CnpjCpf,
                ContatoEmail = clienteEntity.ContatoEmail,
                ContatoNome = clienteEntity.ContatoNome,
                RazaoSocial = clienteEntity.RazaoSocial,
                Id = 0,
                InscricaoEstadual = clienteEntity.InscricaoEstadual,
                NomeFantasia = clienteEntity.NomeFantasia,
                TelefoneDDD = clienteEntity.TelefoneDDD,
                TelefoneNumero = clienteEntity.TelefoneNumero,
                TipoInscricaoEstadual = clienteEntity.TipoInscricaoEstadual.ToString(),
                Enderecos = [],
                EnderecoCobrancaIgualPrincipal = clienteEntity.EnderecoCobrancaIgualPrincipal == "S",
                EnderecoEntregaIgualPrincipal = clienteEntity.EnderecoEntregaIgualPrincipal == "S",
                FretePorConta = clienteEntity.FretePorConta
            };

            return clienteModel;
        }
        return null;
    }
    
}

public record RetornaClienteWebCNPJQuery : IRequest<ClienteWebModel?>
{
    public required string CnpjCpf { get; init; }
}