using BlessWebPedidoSidi.Application.Cliente.Pesquisa;
using BlessWebPedidoSidi.Application.Shared;
using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace BlessWebPedidoSidi.Application.ClientesWeb.GravaClienteWeb;

public class GravaClienteWebHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<GravaClienteWebCommand, int>
{
    private async Task ValidaDados(ClienteWebEntity clienteWebEntity, GravaClienteWebCommand command)
    {
        var cnpjCpf = Regex.Replace(command.CnpjCpf, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        var validaCnpjCpfCommand = new ValidaCnpjCpfCommand() { CnpjCpf = cnpjCpf };
        await mediator.Send(validaCnpjCpfCommand);

        if (command.Id == 0)
        {
            var pesquisaClienteQuery = new PesquisaClienteQuery()
            {
                CnpjCpf = cnpjCpf,
                RepresentanteCnpj = command.RepresentanteCnpj,
                EmpresaCnpj = command.EmpresaCnpj,
                CnpjOuNome = "",
                UsuarioCodigo = command.UsuarioCodigo,
                ApenasClientesDoRepresentante = false,
                Pagina=1,
                RegistrosPorPagina=10
            };

            var clienteJaExiste = await mediator.Send(pesquisaClienteQuery);

            if (clienteJaExiste.Registros.Count > 0)
            {
                if (clienteJaExiste.Registros.First().Origem == "ERP")
                {
                    throw new BadHttpRequestException("GCH09 - Já existe um cliente cadastrado com esse cnpj. Tente realizar uma pesquisa, caso ele não apareça, entre em contato com a fábrica.");
                }
                else
                {
                    throw new BadHttpRequestException("GCH02 - Já existe um cliente cadastrado com esse cnpj");
                }
            }
        }

        if (clienteWebEntity.CnpjCpf.IsNullOrEmpty())
            throw new BadHttpRequestException("GCH03 - O campo CNPJ/Cpf deve ser preenchido");

        if (clienteWebEntity.RazaoSocial.IsNullOrEmpty())
            throw new BadHttpRequestException("GCH04 - O campo Razão Social deve ser preenchido");

        if (clienteWebEntity.ContatoNome.IsNullOrEmpty())
            throw new BadHttpRequestException("GCH05 - O campo Nome do Contato deve ser preenchido");

        if (clienteWebEntity.ContatoEmail.IsNullOrEmpty())
            throw new BadHttpRequestException("GCH06 - O campo Email do Contato deve ser preenchido");

        if (clienteWebEntity.CelularDDD.IsNullOrEmpty())
            throw new BadHttpRequestException("GCH07 - O campo Celular DDD deve ser preenchido");

        if (clienteWebEntity.CelularNumero.IsNullOrEmpty())
            throw new BadHttpRequestException("GCH08 - O campo Numero do Celular deve ser preenchido");
    }

    public async Task<int> Handle(GravaClienteWebCommand command, CancellationToken cancellationToken)
    {
        var clienteEntity = CriaNovoCliente(command);

        if (command.Id == 0)
        {
            foreach (var enderecoCommand in command.Enderecos)
            {
                ClienteWebEnderecoEntity clienteEnderecoEntity = await CriarNovoEndereco(clienteEntity, enderecoCommand);
                clienteEntity.Enderecos.Add(clienteEnderecoEntity);
            }
        }
        else
        {
            clienteEntity = await AtualizaCliente(command);
            AtualizaEnderecos(command, clienteEntity);

            foreach (var enderecoCommand in command.Enderecos)
                await InsereNovosEnderecos(clienteEntity, enderecoCommand);
        }

        var enderecoPrincipal = clienteEntity.Enderecos.Where(x => x.Tipo == ETipoEndereco.Principal).First();

        if (command.EnderecoEntregaIgualPrincipal == "S")
        {
            foreach (var endereco in clienteEntity.Enderecos)
            {
                if (endereco.Tipo == ETipoEndereco.Entrega)
                {
                    endereco.Rua = enderecoPrincipal.Rua;
                    endereco.Bairro = enderecoPrincipal.Bairro;
                    endereco.Numero = enderecoPrincipal.Numero;
                    endereco.Complemento = enderecoPrincipal.Complemento;
                    endereco.Cidade = enderecoPrincipal.Cidade;
                    endereco.CidadeCodigo = enderecoPrincipal.CidadeCodigo;
                    endereco.Cep = enderecoPrincipal.Cep;
                }
            }
        }

        if (command.EnderecoCobrancaIgualPrincipal == "S")
        {
            foreach (var endereco in clienteEntity.Enderecos)
            {
                if (endereco.Tipo == ETipoEndereco.Cobranca)
                {
                    endereco.Rua = enderecoPrincipal.Rua;
                    endereco.Bairro = enderecoPrincipal.Bairro;
                    endereco.Numero = enderecoPrincipal.Numero;
                    endereco.Complemento = enderecoPrincipal.Complemento;
                    endereco.Cidade = enderecoPrincipal.Cidade;
                    endereco.CidadeCodigo = enderecoPrincipal.CidadeCodigo;
                    endereco.Cep = enderecoPrincipal.Cep;
                }
            }
        }

        await ValidaDados(clienteEntity, command);
        await unitOfWork.StartTransactionAsync();
        try
        {
            await unitOfWork.ClienteWebRepository.GravaAsync(clienteEntity, command.Id);

            foreach (var enderecoId in command.EnderecosIdsExcluidos)
                await unitOfWork.ClienteWebEnderecoRepository.ExcluirAsync(
                    x => x.Id == enderecoId
                    && x.ClienteId == clienteEntity.Id
                );

            await unitOfWork.CommitTransactionAsync();
            return clienteEntity.Id;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new BadHttpRequestException("GCH01 - Falha ao gravar o cliente", ex);
        }

    }

    private async Task InsereNovosEnderecos(ClienteWebEntity clienteWebEntity, GravaClienteWebEnderecoCommand enderecoCommand)
    {
        var enderecoCliente = clienteWebEntity.Enderecos.Where(x => x.Tipo == enderecoCommand.Tipo).FirstOrDefault();
        if (enderecoCliente == null)
        {
            enderecoCliente = await CriarNovoEndereco(clienteWebEntity, enderecoCommand);
            clienteWebEntity.Enderecos.Add(enderecoCliente);
        }
    }

    private static ClienteWebEntity CriaNovoCliente(GravaClienteWebCommand command)
    {
        return new ClienteWebEntity()
        {
            Id = command.Id,
            CelularDDD = command.CelularDDD,
            CelularNumero = command.CelularNumero,
            CnpjCpf = new string(command.CnpjCpf.Where(char.IsDigit).ToArray()),
            ContatoEmail = command.ContatoEmail,
            ContatoNome = command.ContatoNome,
            EmpresaCnpj = command.EmpresaCnpj,
            RazaoSocial = command.RazaoSocial,
            RepresentanteCnpj = command.RepresentanteCnpj,
            InscricaoEstadual = command.InscricaoEstadual,
            NomeFantasia = command.NomeFantasia,
            TelefoneDDD = command.TelefoneDDD,
            TelefoneNumero = command.TelefoneNumero,
            TipoInscricaoEstadual = command.TipoInscricaoEstadual,
            EnderecoCobrancaIgualPrincipal = command.EnderecoCobrancaIgualPrincipal,
            EnderecoEntregaIgualPrincipal = command.EnderecoEntregaIgualPrincipal,
            FretePorConta = command.FretePorConta,
            Sincronizado = "N"
        };
    }

    private async Task<ClienteWebEntity> AtualizaCliente(GravaClienteWebCommand command)
    {
        var clienteEntity = await unitOfWork.ClienteWebRepository.CarregarDadosAsync(
            x => x.Id == command.Id && x.RepresentanteCnpj == command.RepresentanteCnpj
            && x.EmpresaCnpj == command.EmpresaCnpj);

        clienteEntity.CelularDDD = command.CelularDDD;
        clienteEntity.CelularNumero = command.CelularNumero;
        clienteEntity.ContatoEmail = command.ContatoEmail;
        clienteEntity.ContatoNome = command.ContatoNome;
        clienteEntity.EmpresaCnpj = command.EmpresaCnpj;
        clienteEntity.RazaoSocial = command.RazaoSocial;
        clienteEntity.RepresentanteCnpj = command.RepresentanteCnpj;
        clienteEntity.InscricaoEstadual = command.InscricaoEstadual;
        clienteEntity.NomeFantasia = command.NomeFantasia;
        clienteEntity.TelefoneDDD = command.TelefoneDDD;
        clienteEntity.TelefoneNumero = command.TelefoneNumero;
        clienteEntity.TipoInscricaoEstadual = command.TipoInscricaoEstadual;
        clienteEntity.EnderecoCobrancaIgualPrincipal = command.EnderecoCobrancaIgualPrincipal;
        clienteEntity.EnderecoEntregaIgualPrincipal = command.EnderecoEntregaIgualPrincipal;
        clienteEntity.FretePorConta = command.FretePorConta;
        return clienteEntity;
    }

    private async Task<ClienteWebEnderecoEntity> CriarNovoEndereco(ClienteWebEntity clienteWebEntity, GravaClienteWebEnderecoCommand enderecoCommand)
    {
        var cidadeWebEntity = (await unitOfWork.CidadeRepository.PesquisaAsync(
            x => x.Codigo == enderecoCommand.CidadeId, false)).FirstOrDefault()
            ?? throw new BadHttpRequestException($"GCH09 - Cidade não encontrada com o id {enderecoCommand.CidadeId}");

        return new ClienteWebEnderecoEntity()
        {
            Bairro = enderecoCommand.Bairro,
            Cep = enderecoCommand.Cep,
            ClienteWeb = clienteWebEntity,
            ClienteId = clienteWebEntity.Id,
            Complemento = enderecoCommand.Complemento,
            Numero = enderecoCommand.Numero,
            Rua = enderecoCommand.Rua,
            CidadeCodigo = enderecoCommand.CidadeId,
            Tipo = enderecoCommand.Tipo,
            Id = 0,
            Cidade = cidadeWebEntity!,

        };
    }

    private static void AtualizaEnderecos(GravaClienteWebCommand command, ClienteWebEntity clienteWebEntity)
    {
        foreach (var clienteEnderecoEntity in clienteWebEntity.Enderecos)
        {
            var enderecoAtualizaCommand = command.Enderecos
                .Where(x => x.Tipo == clienteEnderecoEntity.Tipo).FirstOrDefault();
            if (enderecoAtualizaCommand != null)
            {
                clienteEnderecoEntity.Bairro = enderecoAtualizaCommand.Bairro;
                clienteEnderecoEntity.Numero = enderecoAtualizaCommand.Numero;
                clienteEnderecoEntity.Cep = enderecoAtualizaCommand.Cep;
                clienteEnderecoEntity.Rua = enderecoAtualizaCommand.Rua;
                clienteEnderecoEntity.CidadeCodigo = enderecoAtualizaCommand.CidadeId;
            }
        }
    }
}
public record GravaClienteWebCommand : IRequest<int>
{
    public int Id { get; set; }
    public required string CnpjCpf { get; set; }
    public string? InscricaoEstadual { get; set; }
    public ETipoInscricaoEstadual? TipoInscricaoEstadual { get; set; }
    public required string RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required string EmpresaCnpj { get; set; }
    public string? TelefoneDDD { get; set; }
    public string? TelefoneNumero { get; set; }
    public required string CelularDDD { get; set; }
    public required string CelularNumero { get; set; }
    public required string ContatoNome { get; set; }
    public required string ContatoEmail { get; set; }
    public required string EnderecoEntregaIgualPrincipal { get; set; }
    public required string EnderecoCobrancaIgualPrincipal { get; set; }
    public required IList<GravaClienteWebEnderecoCommand> Enderecos { get; set; }
    public required IList<int> EnderecosIdsExcluidos { get; set; }
    public required string FretePorConta { get; set; }
    public required int UsuarioCodigo { get; set; }
}
public record GravaClienteWebEnderecoCommand
{
    public required string Rua { get; set; }
    public required string Numero { get; set; }
    public required string Complemento { get; set; }
    public required string Bairro { get; set; }
    public required string Cep { get; set; }
    public required int CidadeId { get; set; }
    public required ETipoEndereco Tipo { get; set; }
}