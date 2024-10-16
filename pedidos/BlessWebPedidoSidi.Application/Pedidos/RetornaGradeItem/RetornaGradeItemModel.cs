namespace BlessWebPedidoSidi.Application.Pedidos.RetornaGradeItem;

public record GradeItemModel
{
    public required String Tamanho { get; set; }
    public required String TamanhoDescricao { get; set; }
    public int Quantidade { get; set; }
    public int Faturado { get; set; }
    public int EmAberto { get; set; }
}

