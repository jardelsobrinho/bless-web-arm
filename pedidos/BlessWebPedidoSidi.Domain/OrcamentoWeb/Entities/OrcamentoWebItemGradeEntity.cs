using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;

public class OrcamentoWebItemGradeEntity : IEntity
{
    public required int Id { get; set; }
    public required int OrcamentoItemId { get; set; }
    public required string TamanhoCodigo { get; set; }
    public required string TamanhoDescricao { get; set; }
    public required double Quantidade { get; set; }    
    public required OrcamentoWebItemEntity OrcamentoItem { get; set; }
}