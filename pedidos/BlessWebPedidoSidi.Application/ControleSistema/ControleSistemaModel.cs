namespace BlessWebPedidoSidi.Application.ControleSistema;

public record ControleSistemaModel
{
    public required string UsarPrazoMedio { get; set; }
    public required string GradeEspecialPedido { get; set; }
    public int FabricaPadrao { get; set; }
    public string? EmpresaCnpj { get; set; }
    public string? ExibirPeca { get; set; }

}