using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class ClienteWebEnderecoMap : IEntityTypeConfiguration<ClienteWebEnderecoEntity>
{
    public void Configure(EntityTypeBuilder<ClienteWebEnderecoEntity> builder)
    {
        builder.ToTable("WEB_CLIENTE_ENDERECO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Rua).HasColumnName("RUA");
        builder.Property(x => x.Numero).HasColumnName("NUMERO");
        builder.Property(x => x.Complemento).HasColumnName("COMPLEMENTO");
        builder.Property(x => x.Bairro).HasColumnName("BAIRRO");
        builder.Property(x => x.Cep).HasColumnName("CEP");

        builder.Property(x => x.Tipo).HasColumnName("TIPO")
            .HasConversion(
                v => v.ToString(),
                v => (ETipoEndereco)Enum.Parse(typeof(ETipoEndereco), v));

        builder.Property(x => x.CidadeCodigo).HasColumnName("CIDADE_CODIGO");
        builder.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID");

        builder.HasOne(x => x.ClienteWeb).WithMany(x => x.Enderecos).HasForeignKey(x => x.ClienteId);
        builder.HasOne(x => x.Cidade).WithMany().HasForeignKey(x => x.CidadeCodigo);
    }
}