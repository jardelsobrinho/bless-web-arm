using BlessWebPedidoSidi.Domain.Modelos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class ModeloMap : IEntityTypeConfiguration<ModeloEntity>
{
    public void Configure(EntityTypeBuilder<ModeloEntity> builder)
    {
        builder.ToTable("MODELOS");
        builder.HasKey(p => p.Codigo);

        builder.Property(x => x.Codigo)
            .HasColumnType("INTEGER")
            .HasColumnName("MODELO");


        builder.Property(x => x.Descricao)
            .HasColumnType("VARCHAR(100)")
            .HasColumnName("DESCRICAO");

        builder.Property(x => x.Referencia)
            .HasColumnType("INTEGER")
            .HasColumnName("FK_LINHA");

        builder.Property(x => x.PrecoVenda)
            .HasColumnType("DOUBLE_PRECISION")
            .HasColumnName("PRECO_VENDA");

        builder.Property(x => x.NomeFoto)
            .HasColumnType("VARCHAR(255)")
            .HasColumnName("NOMEFOTO");
    }
}
