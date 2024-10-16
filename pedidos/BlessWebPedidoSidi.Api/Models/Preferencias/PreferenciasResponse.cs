namespace BlessWebPedidoSidi.Api.Models.Preferencias;

public record PreferenciasResponse
{
    public required string UsarPrazoMedio { get; set; }
    public required string GradeEspecialPedido { get; set; }
    public int FabricaPadrao { get; set; }
    public string? EmpresaCnpj { get; set; }
    public string? ExibirPeca { get; set; }
    public required string SelecionaTabelaPreco { get; set; }
    public required string FiltrarPrazoMedioCondicoes { get; set; }
    public required string PastaAwsS3 { get; set; }
    public required string ValidaEstoqueAcabadoSibMobile { get; set; }
    public required string ValidaEstoqueDisponivel { get; set; }
    public required int DiaPrimeiraQuinzena { get; set; }
    public required int DiaSegundaQuinzena { get; set; }
    public required string PreenchePrevisaoEntrega { get; set; }
    public required string ExibirPrsMultiploGradeWeb { get; set; }
    public required string MarcaPedido { get; set; } = "F";
    public required string PermitirCoresTabelaPreco { get; set; }
    public string? PreencherPrevisaoEntrega { get; set; }

}
