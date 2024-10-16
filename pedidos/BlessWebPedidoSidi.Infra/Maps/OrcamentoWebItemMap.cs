using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class OrcamentoWebItemMap : IEntityTypeConfiguration<OrcamentoWebItemEntity>
{
    public void Configure(EntityTypeBuilder<OrcamentoWebItemEntity> builder)
    {
        builder.ToTable("WEB_ORCAMENTO_ITEM");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.OrcamentoId).HasColumnName("ORCAMENTO_ID");
        builder.Property(x => x.ModeloCodigo).HasColumnName("MODELO_CODIGO");
        builder.Property(x => x.CorCodigo).HasColumnName("COR_CODIGO");
        builder.Property(x => x.ModeloDescricao).HasColumnName("MODELO_DESCRICAO");
        builder.Property(x => x.CorDescricao).HasColumnName("COR_DESCRICAO");
        builder.Property(x => x.TotalPares).HasColumnName("TOTAL_PARES");
        builder.Property(x => x.PrecoUnitario).HasColumnName("PRECO_UNITARIO");
        builder.Property(x => x.QuantCaixas).HasColumnName("QUANT_CAIXAS");
        builder.Property(x => x.MarcaCodigo).HasColumnName("MARCA_CODIGO");
        builder.Property(x => x.SoladoCodigo).HasColumnName("SOLADO_ITEM_ESTOQUE_CODIGO");
        builder.Property(x => x.PalmilhaCodigo).HasColumnName("PALMILHA_ITEM_ESTOQUE_CODIGO");
        builder.Property(x => x.ObservacaoItem).HasColumnName("OBS");
        builder.HasOne(x => x.Orcamento).WithMany(x => x.Itens).HasForeignKey(x => x.OrcamentoId);
    }
}