using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.Modelos.Entities;

public class ModeloEntity : IEntity
{
    public required int Codigo { get; set; }

    public required string Descricao { get; set; }

    public required int Referencia { get; set; }

    public required double PrecoVenda { get; set; }

    public  string? NomeFoto { get; set; }
}
