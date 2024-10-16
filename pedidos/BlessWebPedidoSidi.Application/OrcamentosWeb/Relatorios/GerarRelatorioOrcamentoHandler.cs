using BlessWebPedidoSidi.Application.Cliente;
using BlessWebPedidoSidi.Application.Cliente.Pesquisa;
using BlessWebPedidoSidi.Application.Cliente.RetornaRepresentante;
using BlessWebPedidoSidi.Application.ControleSistema;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;
using BlessWebPedidoSidi.Application.Shared.EnviarEmail;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace BlessWebPedidoSidi.Application.OrcamentosWeb.Relatorios;

public record GerarRelatorioOrcamentoQuery : IRequest<Unit>
{
    public required string Uuid { get; set; }
    public required string RepresentanteCnpj { get; set; }
    public required int UsuarioCodigo { get; set; }
    public required EOrcamentoStatus StatusOrcamento { get; set; }
}
public class GerarRelatorioOrcamentoHandler(IMediator mediator) : IRequestHandler<GerarRelatorioOrcamentoQuery, Unit>
{
    public static byte[] GeneratePdf(OrcamentoModel orcamentos, IList<ClienteModel> cliente, RepresentanteModel representante, ControleSistemaModel preferencias)
    {
        var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        try
        {
            document.SetMargins(50, 50, 50, 50);
            var tituloRelatorio = new Paragraph("Orçamento")
                .SetFontSize(25)
                .SetBold();

            var estiloParagrafo = new Style()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetFontSize(10);

            var tituloParagrafoCliente = new Paragraph("Informações do cliente")
                .AddStyle(estiloParagrafo);

            var tituloParagrafoPedido = new Paragraph("Informações do pedido")
                .AddStyle(estiloParagrafo);

            var tipoFrete = orcamentos.Frete switch
            {
                "CIF" => "Emitente",
                "FOB" => "Destinatario",
                _ => "",
            };

            var dataEntrega = new DateTime();
            if (orcamentos.DataEntrega != null)
            {
                dataEntrega = (DateTime)orcamentos.DataEntrega;
            }

            string descricaoCnpjOuCpf = orcamentos.ClienteCnpjCpf!.Length > 11 ? "CNPJ" : "CPF";
            document.Add(tituloRelatorio);
            document.Add(tituloParagrafoCliente);
            document.Add(new Paragraph($"{descricaoCnpjOuCpf}: {orcamentos.ClienteCnpjCpf ?? "N/A"}"));
            document.Add(new Paragraph($"Cliente: {orcamentos.ClienteNome ?? "N/A"}"));

            foreach (var endereco in cliente)
            {
                document.Add(new Paragraph($"Endereço: {endereco.Endereco ?? "N/A"}"));
                document.Add(new Paragraph($"Bairro: {endereco.Bairro ?? "N/A"}"));
                document.Add(new Paragraph($"Cidade: {endereco.Cidade ?? "N/A"}"));
                document.Add(new Paragraph($"UF: {endereco.UF ?? "N/A"}"));
            }

            document.Add(tituloParagrafoPedido);
            document.Add(new Paragraph($"Representante: {representante.RazaoSocial ?? "N/A"}"));
            document.Add(new Paragraph($"Data de Emissão: {orcamentos.DataCriacao.ToString("dd/MM/yyyy") ?? "N/A"}"));
            document.Add(new Paragraph($"Previsão de entrega: {dataEntrega.ToString("dd/MM/yyyy") ?? "N/A"}"));
            document.Add(new Paragraph($"Condição pagt.: {orcamentos.CondicaoPagamentoDescricao ?? "N/A"}"));
            document.Add(new Paragraph($"Frete: {tipoFrete}"));
            document.Add(new Paragraph($"Valor Total: {orcamentos.ValorTotal:C2}"));
            AddItensTable(document, orcamentos.Itens, preferencias);
        }
        catch (Exception ex)
        {
            File.AppendAllText("error.log", $"{DateTime.Now}: Erro ao enviar e-mail - {ex.Message}\n");
            throw new BadHttpRequestException("Ocorreu um erro inesperado ao enviar o e-mail. Por favor, entre em contato com o suporte.");
        }
        finally
        {
            document.Close();
            writer.Close();
        }
        return stream.ToArray();
    }
    private static void AddItensTable(Document document, IList<OrcamentoItemModel> itens, ControleSistemaModel preferencias)
    {
        string exibirPeca = preferencias.ExibirPeca == "S" ? "Peças" : "Pares";

        var estiloHeader = new Style()
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER);

        var estiloCelula = new Style()
            .SetFontSize(8)
            .SetTextAlignment(TextAlignment.CENTER);

        var table = new Table(8)
            .SetWidth(UnitValue.CreatePercentValue(100));

        table.AddHeaderCell("Foto").AddStyle(estiloHeader);
        table.AddHeaderCell("Ref.").AddStyle(estiloHeader);
        table.AddHeaderCell("Produto").AddStyle(estiloHeader);
        table.AddHeaderCell("Cor").AddStyle(estiloHeader);
        table.AddHeaderCell("Grade").AddStyle(estiloHeader);
        table.AddHeaderCell("Vr. Unit").AddStyle(estiloHeader);
        table.AddHeaderCell(exibirPeca).AddStyle(estiloHeader);
        table.AddHeaderCell("Vr. Total").AddStyle(estiloHeader);

        foreach (var item in itens)
        {
            var imageCell = new Cell();
            try
            {
                var imageData = ImageDataFactory.Create(item.Imagem);
                var image = new Image(imageData)
                    .SetMaxHeight(35f)
                    .SetMaxWidth(35f);

                imageCell = new Cell().Add(image).AddStyle(estiloCelula);
            }
            catch (WebException ex)
            {
                var imagemNaoEncontrada = new Paragraph("Imagem não disponível")
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.CENTER);

                if (ex.Message.Contains("(404) Not found"))
                {
                    imageCell = new Cell().Add(imagemNaoEncontrada).AddStyle(estiloCelula);
                }
            }

            table.AddCell(imageCell);
            table.AddCell(item.ReferenciaModelo.ToString()).AddStyle(estiloCelula);
            table.AddCell(item.ModeloDescricao).AddStyle(estiloCelula);
            table.AddCell(item.CorDescricao).AddStyle(estiloCelula);

            var gradeTable = new Table(item.Grade.Count * 2)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetBorder(Border.NO_BORDER);

            foreach (var itemGrade in item.Grade)
            {
                gradeTable.AddCell(new Paragraph(itemGrade.TamanhoDescricao + "\n" + itemGrade.Quantidade)
                    .SetFontSize(8));
            }

            var valorTotal = item.TotalPares * item.PrecoUnitario;

            table.AddCell(gradeTable);
            table.AddCell(item.PrecoUnitario.ToString("C2")).AddStyle(estiloCelula);
            table.AddCell(item.TotalPares.ToString()).AddStyle(estiloCelula);
            table.AddCell(valorTotal.ToString("C2")).AddStyle(estiloCelula);
        }
        document.Add(table);
    }
    public async Task<Unit> Handle(GerarRelatorioOrcamentoQuery command, CancellationToken cancellationToken)
    {
        var controleSistemaQuery = new ControleSistemaQuery();
        var controleSistema = await mediator.Send(controleSistemaQuery, cancellationToken);

        var orcamentoQuery = new RetornaOrcamentoQuery()
        {
            RepresentanteCnpj = command.RepresentanteCnpj,
            UsuarioCodigo = command.UsuarioCodigo,
            Uuid = command.Uuid,
            Status = command.StatusOrcamento
        };

        var orcamentos = await mediator.Send(orcamentoQuery, cancellationToken);
        var clienteQuery = new PesquisaClienteQuery()
        {
            CnpjCpf = orcamentos.ClienteCnpjCpf!,
            CnpjOuNome = "",
            EmpresaCnpj = controleSistema.EmpresaCnpj!,
            RepresentanteCnpj = command.RepresentanteCnpj,
            UsuarioCodigo = command.UsuarioCodigo,
            ApenasClientesDoRepresentante = true,
            Pagina = 1,
            RegistrosPorPagina = 10
        };

        var clientePesquisa = await mediator.Send(clienteQuery, cancellationToken);

        var representanteQuery = new RetornarDadosRepresentanteQuery()
        {
            RepresentanteCNPJ = command.RepresentanteCnpj
        };
        var representante = await mediator.Send(representanteQuery, cancellationToken);

        var pdfBytes = GeneratePdf(orcamentos, clientePesquisa.Registros, representante, controleSistema);
        var listaDeEmails = new List<string>();

        foreach (var item in clientePesquisa.Registros)
        {
            if (!string.IsNullOrEmpty(item.ContatoEmail))
            {
                listaDeEmails.Add(item.ContatoEmail);
            }
            else
            {
                throw new BadHttpRequestException("E-mail do cliente não está informado em seu cadastro.");
            }
        }

        if (!string.IsNullOrEmpty(representante.ContatoEmail))
        {
            listaDeEmails.Add(representante.ContatoEmail);
        }
        else
        {
            throw new BadHttpRequestException("Email do representante não esta informado em seu cadastro.");
        }

        foreach (var emails in listaDeEmails)
        {
            var queryEmail = new EnviarEmailComAnexoCommand()
            {
                Anexo = pdfBytes,
                Assunto = "Orçamento",
                Corpo = "Resumo do orçamento",
                Destinatario = emails,
                NomeAnexo = "Orcamento.pdf",
            };
            await mediator.Send(queryEmail, cancellationToken);
        }
        return Unit.Value;
    }
}