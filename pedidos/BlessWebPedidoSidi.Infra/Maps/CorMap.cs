using BlessWebPedidoSidi.Domain.Modelos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class CorMap : IEntityTypeConfiguration<CorEntity>
{
    public void Configure(EntityTypeBuilder<CorEntity> builder)
    {
        builder.ToTable("FICHA_TECNICA_HD");
        builder.HasKey(p => new { p.Codigo, p.ModeloCodigo });

        builder.Property(x => x.Codigo)
            .HasColumnType("INTEGER")
            .HasColumnName("VERSAO");

        builder.Property(x => x.ModeloCodigo)
                    .HasColumnType("INTEGER")
                    .HasColumnName("FK_MODELO");

        builder.Property(x => x.Nome)
            .HasColumnType("VARCHAR(150)")
            .HasColumnName("NOME");
    }
}
