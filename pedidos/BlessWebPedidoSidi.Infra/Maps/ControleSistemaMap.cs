using BlessWebPedidoSidi.Domain.ControleSistema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class ControleSistemaMap : IEntityTypeConfiguration<ControleSistemaEntity>
{
    public void Configure(EntityTypeBuilder<ControleSistemaEntity> builder)
    {
        builder.ToTable("CONTROLE_SISTEMA");
        builder.HasKey(x => x.Cnpj);
        builder.Property(x => x.Cnpj)
            .HasColumnName("CGC")
            .HasColumnType("VARCHAR(14)");
    }
}