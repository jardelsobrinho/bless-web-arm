using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.Modelos.PesquisaModelos;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;
using BlessWebPedidoSidi.Application.OrcamentosWeb.GravaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamentoParaEdicao;
using BlessWebPedidoSidi.Application.Usuario;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.Shared;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens;

public class AdicionaItemHandler(IUnitOfWork unitOfWork, IMediator mediator, IDbConnection conexao) : IRequestHandler<AdicionaItemCommand, Unit>
{
    public async Task<Unit> Handle(AdicionaItemCommand command, CancellationToken cancellationToken)
    {
        var usuario = await mediator.Send(new UsuarioQuery() { UsuarioCodigo = command.UsuarioCodigo });
        if (usuario.ExibirMarcaVendasWeb == "S")
        {
            var produtosSemMarca = command.Cores.Where(x => x.MarcaCodigo == 0 || x.MarcaCodigo == -1).ToList();
            if (produtosSemMarca.Count > 0)
            {
                throw new BadHttpRequestException($"ATH04 - Informe a marca do produto, antes de inserir no orçamento!");
            }
        }

        var controleSistemaQuery = new ControleSistemaQuery();
        var controleSistema = await mediator.Send(controleSistemaQuery, cancellationToken);
        if (controleSistema.ExibirPeca != "S")
        {
            foreach (var cor in command.Cores)
            {
                if (cor.QuantCaixas <= 0)
                    throw new BadHttpRequestException("ATH03 - O Campo caixas deve ser maior que zero!");
            }
        }

        var orcamentoParaEdicaoQuery = new RetornaOrcamentoParaEdicaoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            Uuid = command.Uuid,
            UsuarioCodigo = command.UsuarioCodigo
        };

        var sqlModeloPreferencia = $"SELECT M.VALIDA_GRADE_PEDIDO ValidarGradePedido, M.QUANTIDADE_MINIMA QuantidadeMinima FROM MODELOS M WHERE MODELO = {command.ModeloCodigo} ";
        var preferenciaModelo = (await conexao.QueryAsync<PreferenciaModeloModel>(sqlModeloPreferencia)).FirstOrDefault();

        var orcamento = await mediator.Send(orcamentoParaEdicaoQuery, cancellationToken);
        var tabelaPrecoCodigo = orcamento.TabelaPrecoCodigo ?? 0;
        var condicaoPagamentoCodigo = orcamento.CondicaoPagamentoCodigo ?? 0;

        if (tabelaPrecoCodigo == 0)
            throw new BadHttpRequestException("ATH02 - Informe uma tabela de preço antes de inserir o produto");

        foreach (var cor in command.Cores)
        {
            var queryModelo = new PesquisaModeloQuery
            {
                TabelaPrecoCodigo = tabelaPrecoCodigo,
                CondicaoPagamentoCodigo = condicaoPagamentoCodigo,
                ModeloCodigo = command.ModeloCodigo,
                ReferenciaOuDescricao = "",
                RepresentanteCnpj = command.RepresentanteCnpj,
                Pagina = 1,
                RegistrosPorPagina = 1
            };

            OrcamentoWebItemEntity? orcamentoItem;

            if (cor.Id != null)
            {
                orcamentoItem = orcamento.Itens.Where(x => x.OrcamentoId == orcamento.Id
                    && x.Id == cor.Id).FirstOrDefault();
            }
            else
            {
                orcamentoItem = orcamento.Itens.Where(x => x.OrcamentoId == orcamento.Id
                    && x.CorCodigo == cor.CorCodigo && x.ModeloCodigo == command.ModeloCodigo
                    && x.MarcaCodigo == cor.MarcaCodigo && x.SoladoCodigo == cor.SoladoCodigo
                    && x.PalmilhaCodigo == cor.PalmilhaCodigo && x.ObservacaoItem == cor.ObservacaoItem).FirstOrDefault();
            }

            IList<OrcamentoWebItemGradeEntity>? gradeUpdate = null;

            if (orcamentoItem == null)
            {
                gradeUpdate = new List<OrcamentoWebItemGradeEntity>();
                var modeloEntity = (await unitOfWork.ModeloRepository.PesquisaAsync(x => x.Codigo == command.ModeloCodigo)).First();
                var corEntity = (await unitOfWork.CorRepository.PesquisaAsync(x => x.Codigo == cor.CorCodigo && x.ModeloCodigo == command.ModeloCodigo))
                        .FirstOrDefault() ?? throw new BadHttpRequestException($"AIH01 - Cor {cor.CorCodigo} não encontrado para o modelo {command.ModeloCodigo}");

                orcamentoItem = new OrcamentoWebItemEntity()
                {
                    CorCodigo = cor.CorCodigo,
                    ModeloCodigo = command.ModeloCodigo,
                    ModeloDescricao = modeloEntity.Descricao,
                    CorDescricao = corEntity.Nome,
                    MarcaCodigo = cor.MarcaCodigo,
                    Orcamento = orcamento!,
                    OrcamentoId = orcamento!.Id,
                    PrecoUnitario = 0,
                    QuantCaixas = cor.QuantCaixas,
                    TotalPares = 0,
                    Id = 0,
                    SoladoCodigo = cor.SoladoCodigo,
                    PalmilhaCodigo = cor.PalmilhaCodigo,
                    ObservacaoItem = cor.ObservacaoItem,
                    Grade = gradeUpdate
                };

                foreach (var grade in cor.Grade)
                {
                    var novoOrcamentoItemGrade = CriaNovoItemGrade(orcamentoItem, grade);
                    gradeUpdate.Add(novoOrcamentoItemGrade);
                }

                orcamento.Itens.Add(orcamentoItem);
            }
            else
            {
                orcamentoItem.MarcaCodigo = cor.MarcaCodigo;
                orcamentoItem.SoladoCodigo = cor.SoladoCodigo;
                orcamentoItem.PalmilhaCodigo = cor.PalmilhaCodigo;
                orcamentoItem.ObservacaoItem = cor.ObservacaoItem;

                gradeUpdate = orcamentoItem.Grade;
                foreach (var gradeItem in gradeUpdate)
                {
                    var grade = cor.Grade.Where(x => x.TamanhoCodigo == gradeItem.TamanhoCodigo).FirstOrDefault();
                    if (grade != null)
                    {
                        if (command.SomaQuantidade)
                        {
                            gradeItem.Quantidade += grade.Quantidade;
                        }
                        else
                        {
                            gradeItem.Quantidade = grade.Quantidade;
                        }
                    }
                }

                foreach (var grade in cor.Grade)
                {
                    var gradeEntity = gradeUpdate.Where(x => x.TamanhoCodigo == grade.TamanhoCodigo).FirstOrDefault();
                    if (gradeEntity == null)
                    {
                        var novoOrcamentoItemGrade = CriaNovoItemGrade(orcamentoItem, grade);
                        gradeUpdate.Add(novoOrcamentoItemGrade);
                    }
                }
            }

            orcamentoItem.QuantCaixas = cor.QuantCaixas;
            var totalPares = gradeUpdate.Sum(x => x.Quantidade);

            if (controleSistema.ExibirPeca != "S")
            {
                orcamentoItem.TotalPares = orcamentoItem.QuantCaixas == 0 ? totalPares : totalPares * orcamentoItem.QuantCaixas;
            }
            else
            {
                orcamentoItem.TotalPares = totalPares;
            }
        }

        var gravaOrcamentoCommand = new GravaOrcamentoCommand() { OrcamentoWeb = orcamento };
        await mediator.Send(gravaOrcamentoCommand, cancellationToken);

        return Unit.Value;
    }

    private static OrcamentoWebItemGradeEntity CriaNovoItemGrade(OrcamentoWebItemEntity orcamentoItem, TamanhoModel grade)
    {
        return new OrcamentoWebItemGradeEntity()
        {
            Id = 0,
            OrcamentoItemId = orcamentoItem.Id,
            OrcamentoItem = orcamentoItem,
            Quantidade = grade.Quantidade,
            TamanhoCodigo = grade.TamanhoCodigo,
            TamanhoDescricao = grade.TamanhoCodigo,
        };
    }
}

public record AdicionaItemCommand : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required int ModeloCodigo { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required IList<CorModel> Cores { get; set; }
    public required bool SomaQuantidade { get; set; }
}