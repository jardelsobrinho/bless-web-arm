using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlessWebPedidoSidi.Application.ClientesWeb.RetornaClienteWebPorId;

public class RetornaClienteWebPorIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<RetornaClienteWebPorIdQuery, ClienteWebModel>
{
    public async Task<ClienteWebModel> Handle(RetornaClienteWebPorIdQuery query, CancellationToken cancellationToken)
    {
        var clienteEntity = (await unitOfWork.ClienteWebRepository.PesquisaAsync(
            x => x.Id == query.ClienteId
                && x.RepresentanteCnpj == query.RepresentanteCnpj
                && x.EmpresaCnpj == query.EmpresaCnpj
            )).SingleOrDefault() ??
            throw new BadHttpRequestException($"RCPIH01 - Cliente não encontrado com o Id {query.ClienteId}");

        var listaEnderecosEntities = (await unitOfWork.ClienteWebEnderecoRepository.PesquisaAsync(
                x => x.ClienteId == query.ClienteId)).ToList();

        var listaEnderecos = new List<ClienteWebEnderecoModel>();
        foreach (var endereco in listaEnderecosEntities)
        {
            var cidadeEntity = (await unitOfWork.CidadeRepository.PesquisaAsync(x => x.Codigo == endereco.CidadeCodigo)).Single();
            var enderecoModel = new ClienteWebEnderecoModel()
            {
                Id = endereco.Id,
                Bairro = endereco.Bairro,
                Cep = endereco.Cep,
                Complemento = endereco.Complemento,
                Numero = endereco.Numero,
                Rua = endereco.Rua,
                CidadeId = endereco.CidadeCodigo,
                Tipo = endereco.Tipo.ToString(),
                UfSigla = cidadeEntity.UfSigla
            };
            listaEnderecos.Add(enderecoModel);
        }

        var clienteModel = new ClienteWebModel()
        {
            CelularDDD = clienteEntity.CelularDDD,
            CelularNumero = clienteEntity.CelularNumero,
            CnpjCpf = clienteEntity.CnpjCpf,
            ContatoEmail = clienteEntity.ContatoEmail,
            ContatoNome = clienteEntity.ContatoNome,
            RazaoSocial = clienteEntity.RazaoSocial,
            Id = query.ClienteId,
            InscricaoEstadual = clienteEntity.InscricaoEstadual,
            NomeFantasia = clienteEntity.NomeFantasia,
            TelefoneDDD = clienteEntity.TelefoneDDD,
            TelefoneNumero = clienteEntity.TelefoneNumero,
            TipoInscricaoEstadual = clienteEntity.TipoInscricaoEstadual.ToString(),
            Enderecos = listaEnderecos,
            EnderecoCobrancaIgualPrincipal = clienteEntity.EnderecoCobrancaIgualPrincipal == "S",
            EnderecoEntregaIgualPrincipal = clienteEntity.EnderecoEntregaIgualPrincipal == "S",
            FretePorConta = clienteEntity.FretePorConta
        };

        return clienteModel;
    }
}

public record RetornaClienteWebPorIdQuery : IRequest<ClienteWebModel>
{
    public required int ClienteId { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
}