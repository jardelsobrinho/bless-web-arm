using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class OrcamentoWebItemGradeMap : IEntityTypeConfiguration<OrcamentoWebItemGradeEntity>
{
    public void Configure(EntityTypeBuilder<OrcamentoWebItemGradeEntity> builder)
    {
        builder.ToTable("WEB_ORCAMENTO_ITEM_GRADE");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.OrcamentoItemId).HasColumnName("ORCAMENTO_ITEM_ID");
        builder.Property(x => x.TamanhoCodigo).HasColumnName("TAMANHO_CODIGO");
        builder.Property(x => x.TamanhoDescricao).HasColumnName("TAMANHO_DESCRICAO");
        builder.Property(x => x.Quantidade).HasColumnName("QUANTIDADE");
        builder.HasOne(x => x.OrcamentoItem).WithMany(x => x.Grade).HasForeignKey(x => x.OrcamentoItemId);
    }
}