using BlessWebPedidoSidi.Domain.Modelos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class MarcaMap : IEntityTypeConfiguration<MarcaEntity>
{
    public void Configure(EntityTypeBuilder<MarcaEntity> builder)
    {
        builder.ToTable("MARCA");
        builder.HasKey(p => new { p.Codigo });

        builder.Property(x => x.Codigo)
            .HasColumnType("INTEGER")
            .HasColumnName("CODIGO");

        builder.Property(x => x.Nome)
            .HasColumnType("VARCHAR(40)")
            .HasColumnName("NOME_MARCA");
    }
}
