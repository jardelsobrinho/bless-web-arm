using BlessWebPedidoSidi.Domain.Ufs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class UfMap : IEntityTypeConfiguration<UfEntity>
{
    public void Configure(EntityTypeBuilder<UfEntity> builder)
    {
        builder.ToTable("UF");
        builder.HasKey(x => x.Sigla);
        builder.Property(x => x.Sigla).HasColumnName("SIGLA_UF");
        builder.Property(x => x.Descricao).HasColumnName("DESCRICAO");
    }
}