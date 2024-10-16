using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class LoginAutomaticoMap : IEntityTypeConfiguration<LoginAutomaticoEntity>
{
    public void Configure(EntityTypeBuilder<LoginAutomaticoEntity> builder)
    {
        builder.ToTable("WEB_LOGIN_AUTOMATICO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Uuid).HasColumnName("UUID");
        builder.Property(x => x.Token).HasColumnName("TOKEN");
        builder.Property(x => x.UrlApi).HasColumnName("URL_API");
        builder.Property(x => x.Usuario).HasColumnName("USUARIO");
        builder.Property(x => x.DataValidade).HasColumnName("DATA_VALIDADE");
    }
}