using BlessWebPedidoSidi.Domain.Cidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class CidadeMap : IEntityTypeConfiguration<CidadeEntity>
{
    public void Configure(EntityTypeBuilder<CidadeEntity> builder)
    {
        builder.ToTable("CIDADE");
        builder.HasKey(x => x.Codigo);
        builder.Property(x => x.Codigo).HasColumnName("CODIGO"); 
        builder.Property(x => x.Nome).HasColumnName("NOME");
        builder.Property(x => x.CodigoIbge).HasColumnName("CODIGO_IBGE");
        builder.Property(x => x.UfSigla).HasColumnName("UF");
        builder.HasOne(x => x.Uf).WithMany(x => x.Cidades).HasForeignKey(x => x.UfSigla);
    }
}