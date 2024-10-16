using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;

public class OrcamentoWebItemEntity : IEntity
{
    public required int Id { get; set; }
    public required int OrcamentoId { get; set; }
    public required int ModeloCodigo { get; set; }
    public required int CorCodigo { get; set; }
    public required string ModeloDescricao { get; set; }
    public required string CorDescricao { get; set; }
    public required double TotalPares { get; set; }
    public required double PrecoUnitario { get; set; }    
    public required int QuantCaixas { get; set; }
    public int? SoladoCodigo { get; set; }
    public int? PalmilhaCodigo { get; set; }
    public int? MarcaCodigo { get; set; }
    public string? ObservacaoItem { get; set; }
    public required OrcamentoWebEntity Orcamento { get; set; }
    public required IList<OrcamentoWebItemGradeEntity> Grade { get; set; } = new List<OrcamentoWebItemGradeEntity>();

}