using Asp.Versioning;
using AutoMapper;
using BlessWebPedidoSidi.Api.Attributes;
using BlessWebPedidoSidi.Api.Models.OrcamentoWeb;
using BlessWebPedidoSidi.Api.Models.OrcamentoWeb.AdicionaModelo;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaCliente;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaCondicaoPagamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaDataEntrega;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaFrete;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaObs;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaTabelaPreco;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AtualizaTipoEstoque;
using BlessWebPedidoSidi.Application.OrcamentosWeb.DuplicaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.ExcluiOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.FinalizaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.NovoOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.PesquisaOrcamentos;
using BlessWebPedidoSidi.Application.OrcamentosWeb.Relatorios;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RemoveItemOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaLogsOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaStatusOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.ValidarQuantidadeEmultiplosGrade;
using BlessWebPedidoSidi.Application.OrcamentosWeb.ValidarQuantidadeEmultiplosGrade.Models;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace BlessWebPedidoSidi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/orcamento-web")]
[Authorize]
[ApiController]
public class OrcamentoWebController(IMediator mediator) : DefaultController
{
    /// <summary>
    /// Cria um novo orçamento
    /// </summary>
    /// <response code="200">Id do novo orçamento</response> 
    [ProducesResponseType(typeof(NovoOrcamentoResponse), StatusCodes.Status200OK)]
    [HttpPost("novo")]
    public async Task<IActionResult> NovoOrcamentoAsync()
    {
        var command = new NovoOrcamentoCommand()
        {
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo
        };

        var uuid = await mediator.Send(command);
        return Ok(new NovoOrcamentoResponse() { Uuid = uuid });
    }

    /// <summary>
    /// Atualiza o cliente do orcamento
    /// </summary>
    /// <response code="204">Cliente atualizado com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-cliente")]
    public async Task<IActionResult> AtualizaClienteAsync([FromRoute] string uuid, [FromBody] AtualizaClienteOrcamentoRequest request)
    {
        var command = new AtualizaClienteOrcamentoCommand()
        {
            Uuid = uuid,
            ClienteCnpjCpf = request.ClienteCnpjCpf,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Atualiza a tabela de preço do orcamento
    /// </summary>
    /// <response code="204">Tabela de preço atualizado com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-tabela-preco")]
    public async Task<IActionResult> AtualizaTabelaPrecoAsync([FromRoute] string uuid, [FromBody] AtualizaTabPrecoOrcamentoRequest request)
    {
        var command = new AtualizaTabPrecoOrcamentoCommand()
        {
            Uuid = uuid,
            TabelaPrecoCodigo = request.TabelaPrecoCodigo,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Duplica um orçamento
    /// </summary>
    /// <response code="201">Retorna uuid do novo orçamento</response> 
    [ProducesResponseType(typeof(DuplicaOrcamentoModel), StatusCodes.Status200OK)]
    [HttpPost("{uuid}/duplica")]
    public async Task<IActionResult> DuplicaOrcamentoAsync([FromRoute] string uuid)
    {
        var command = new DuplicaOrcamentoCommand()
        {
            Uuid = uuid,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Atualiza a data de entrega do orçamento
    /// </summary>
    /// <response code="200">Data de entrega atualizado com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-data-entrega")]
    public async Task<IActionResult> AtualizaDataEntregaAsync([FromRoute] string uuid, [FromBody] AtualizaDataEntregaOrcamentoRequest request)
    {
        AtualizaDataEntregaCommand command = new()
        {
            Uuid = uuid,
            DataEntrega = request.DataEntrega,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj
        };

        var dataEntrega = await mediator.Send(command);
        return Ok(dataEntrega);
    }

    /// <summary>
    /// Atualiza o tipo do estoque
    /// </summary>
    /// <response code="204">Tipo do estoque atualizado com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-tipo-estoque")]
    public async Task<IActionResult> AtualizaTipoEstoqueAsync([FromRoute] string uuid, [FromBody] AtualizaTipoEstoqueOrcamentoRequest request)
    {
        var command = new AtualizaTipoEstoqueCommand()
        {
            Uuid = uuid,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            TipoEstoqueCodigo = request.TipoEstoqueCodigo,
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Atualiza a observação
    /// </summary>
    /// <response code="204">Observação atualizada com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-obs")]
    public async Task<IActionResult> AtualizaObsAsync([FromRoute] string uuid, [FromBody] AtualizaObsOrcamentoRequest request)
    {
        var command = new AtualizaObsCommand()
        {
            Uuid = uuid,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            Obs = request.Obs
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Atualiza frete
    /// </summary>
    /// <response code="204">Frete atualizado com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-frete")]
    public async Task<IActionResult> AtualizaFreteAsync([FromRoute] string uuid, [FromBody] AtualizaFreteOrcamentoRequest request)
    {
        var command = new AtualizaFreteCommand()
        {
            Uuid = uuid,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            Frete = request.Frete
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Atualiza a condição de pagamento do orcamento
    /// </summary>
    /// <response code="204">Condição de pagamento atualizado com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{uuid}/atualiza-condicao-pagamento")]
    public async Task<IActionResult> AtualizaCondicaoPagamentoAsync([FromRoute] string uuid, [FromBody] AtualizaCondPagtoOrcamentoRequest request)
    {
        var command = new AtualizaCondPagtoOrcamentoCommand()
        {
            Uuid = uuid,
            CondicaoPagamentoCodigo = request.CondicaoPagamentoCodigo,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            TabelaPrecoCodigo = request.TabelaPrecoCodigo
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Adiciona modelo e cor ao orcamento
    /// </summary>
    /// <response code="204">Modelo adicionado com sucesso</response> 
    [ProducesResponseType(typeof(AdicionaModeloRequest), StatusCodes.Status200OK)]
    [HttpPost("{uuid}/adiciona-modelo-cor")]
    public async Task<IActionResult> AdicionaModeloCorAsync([FromServices] IMapper mapper, [FromRoute] string uuid, [FromBody] AdicionaModeloRequest request)
    {

        var listaCores = mapper.Map<List<CorModel>>(request.Cores);

        var command = new AdicionaItemCommand()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid,
            ModeloCodigo = request.ModeloCodigo,
            Cores = listaCores,
            SomaQuantidade = request.SomaQuantidade
        };

        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Finaliza o orçamento
    /// </summary>
    /// <response code="200">Orçamento finalizado</response> 
    [ProducesResponseType(typeof(FinalizaOrcamentoRequest), StatusCodes.Status200OK)]
    [HttpPost("{uuid}/finaliza")]
    public async Task<IActionResult> FinalizaAsync([FromRoute] string uuid, [FromBody] FinalizaOrcamentoRequest request)
    {
        var command = new FinalizaOrcamentoCommand()
        {
            Frete = request.Frete,
            Uuid = uuid,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo
        };

        await mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// Retorna orcamentos
    /// </summary>
    /// <remarks>
    /// Exemplo:
    ///
    ///     GET /v1/orcamento
    ///     {
    ///        "status": ["Aberto", "Concluido", "Transmitido"]
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Orcamentos em aberto</response> 
    [ProducesResponseType(typeof(IList<PesquisaOrcamentoModel>), StatusCodes.Status200OK)]
    [HttpGet("pesquisa")]
    public async Task<IActionResult> PesquisaOrcamentosAbertosAsync([FromQuery] PesquisaOrcamentoRequest request)
    {
        var query = new PesquisaOrcamentosQuery()
        {
            EmpresaCnpj = DadosToken.EmpresaCnpj,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Status = (EOrcamentoStatus)Enum.Parse(typeof(EOrcamentoStatus), request.Status),
            PedidoNumeroOuClienteNome = request.PedidoNumeroOuClienteNome ?? "",
            PedidoDataEmissao = request.PedidoDataEmissao
        };

        var listaOrcamentos = await mediator.Send(query);
        return Ok(listaOrcamentos);
    }

    /// <summary>
    /// Retorna os dados do orçamento
    /// </summary>
    /// <response code="200">Dados do orçamento</response> 
    [ProducesResponseType(typeof(OrcamentoModel), StatusCodes.Status200OK)]
    [HttpGet("{uuid}")]
    public async Task<IActionResult> RetornaOrcamentoAsync([FromRoute] string uuid)
    {
        var query = new RetornaOrcamentoQuery()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid,
            ModeloCodigo = 0,
            CorCodigo = 0,
            Status = EOrcamentoStatus.Aberto
        };

        var orcamento = await mediator.Send(query);
        return Ok(orcamento);
    }


    /// <summary>
    /// Retorna os dados do orçamento finalizado
    /// </summary>
    /// <response code="200">Dados do orçamento finalizado</response> 
    [ProducesResponseType(typeof(OrcamentoModel), StatusCodes.Status200OK)]
    [HttpGet("{uuid}/itensPedido")]
    public async Task<IActionResult> RetornaOrcamentoDadosAsync([FromRoute] string uuid)
    {
        var query = new RetornaOrcamentoQuery()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid,
            CorCodigo = 0,
            ModeloCodigo = 0,
            Status = EOrcamentoStatus.Finalizado
        };

        var orcamento = await mediator.Send(query);
        return Ok(orcamento);
    }

    /// <summary>
    /// Retorna o status do orçamento
    /// </summary>
    /// <response code="200">Status do orçamento</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("{uuid}/retorna-status-orcamento")]
    public async Task<IActionResult> RetornaStatusOrcamentoAsync([FromRoute] string uuid)
    {
        var query = new RetornaStatusOrcamentoQuery()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid
        };

        var status = await mediator.Send(query);
        return Ok(new RetornaStatusOrcamentoResponse() { StatusOrcamento = status });
    }

    /// <summary>
    /// Retorna os logs do orçamento
    /// </summary>
    /// <response code="200">Logs do orçamento</response> 
    [ProducesResponseType(typeof(IList<RetornaLogsOrcamentoModel>), StatusCodes.Status200OK)]
    [HttpGet("{uuid}/logs")]
    public async Task<IActionResult> RetornaLogsAsync([FromRoute] string uuid)
    {
        var query = new RetornaLogsOrcamentoQuery()
        {
            Uuid = uuid
        };

        var logs = await mediator.Send(query);
        return Ok(logs);
    }

    /// <summary>
    /// Remove item do orçamento
    /// </summary>
    /// <response code="204">Item removido com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{uuid}/item/{id}")]
    public async Task<IActionResult> RemoveItemAsync([FromRoute] string uuid, int id)
    {
        var query = new RemoveItemOrcamentoCommand()
        {
            OrcamentoItemId = id,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid
        };

        await mediator.Send(query);
        return NoContent();
    }

    /// <summary>
    /// Excluir o orçamento
    /// </summary>
    /// <response code="204">Orçamento com sucesso</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{uuid}")]
    public async Task<IActionResult> ExcluirOrcamentoAbertoAsync([FromRoute] string uuid)
    {
        var query = new ExcluiOrcamentoCommand()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid
        };

        await mediator.Send(query);
        return NoContent();
    }

    /// <summary>
    /// Retorna o modeloCor do orçamento
    /// </summary>
    /// <response code="200">Dados do orçamento</response> 
    [ProducesResponseType(typeof(OrcamentoModel), StatusCodes.Status200OK)]
    [HttpGet("{uuid}/retorna-modelo-cor")]
    public async Task<IActionResult> RetornaProdutoCorOrcamentoAsync([FromRoute] string uuid, [FromQuery] int? modeloCodigo, [FromQuery] int? corCodigo)
    {
        var query = new RetornaOrcamentoQuery()
        {
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            Uuid = uuid,
            ModeloCodigo = modeloCodigo ?? 0,
            CorCodigo = corCodigo ?? 0,
            Status = EOrcamentoStatus.Aberto
        };

        var orcamento = await mediator.Send(query);
        return Ok(orcamento);
    }

    /// <summary>
    /// Gera relatorio em pdf do orçamento
    /// </summary>
    /// <response code="204">Dados do orçamento em pdf</response> 
    [ProducesResponseType(typeof(OrcamentoModel), StatusCodes.Status204NoContent)]
    [HttpPost("{uuid}/gerarRelatorioPdf")]
    public async Task<IActionResult> RetornaRelatorioOrcamentoAsync([FromRoute] string uuid, string statusOrcamento)
    {
        _ = Enum.TryParse(statusOrcamento, out EOrcamentoStatus statusEnum);
        var query = new GerarRelatorioOrcamentoQuery()
        {
            Uuid = uuid,
            RepresentanteCnpj = DadosToken.RepresentanteCnpj,
            UsuarioCodigo = DadosToken.UsuarioCodigo,
            StatusOrcamento = statusEnum
        };

        await mediator.Send(query);
        return NoContent();
    }

    /// <summary>
    /// Validar Quantidade e multiplos na grade
    /// </summary>
    /// <response code="200">Validar Quantidade e multiplos na grade</response> 
    [ProducesResponseType(typeof(ValidarQuantidadeMinimaProdutoModel), StatusCodes.Status200OK)]
    [HttpGet("validarQuantidadeMultiplosGrade")]
    public async Task<IActionResult> ValidarQuantidadeMultiplosProdutoAsync([FromQuery] int codigoModelo, [FromQuery] int quantidadeInformada)
    {
        var query = new ValidarQuantidadeMinimaProdutoQuery()
        {
            QuantidadeInformada = quantidadeInformada,
            ModeloCodigo = codigoModelo
        };

        var listagem = await mediator.Send(query);
        return Ok(listagem);
    }
}
