using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.ClientesWeb.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class ClienteWebMap : IEntityTypeConfiguration<ClienteWebEntity>
{
    public void Configure(EntityTypeBuilder<ClienteWebEntity> builder)
    {
        builder.ToTable("WEB_CLIENTE");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.CnpjCpf).HasColumnName("CNPJ_CPF");
        builder.Property(x => x.InscricaoEstadual).HasColumnName("INSCRICAO_ESTADUAL");
        builder.Property(x => x.TipoInscricaoEstadual).HasColumnName("TIPO_INSCRICAO_ESTADUAL")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => v != null ? (ETipoInscricaoEstadual)Enum.Parse(typeof(ETipoInscricaoEstadual), v!) : null);
        builder.Property(x => x.RazaoSocial).HasColumnName("RAZAO_SOCIAL");
        builder.Property(x => x.NomeFantasia).HasColumnName("NOME_FANTASIA");
        builder.Property(x => x.RepresentanteCnpj).HasColumnName("REPRESENTANTE_CNPJ");
        builder.Property(x => x.EmpresaCnpj).HasColumnName("EMPRESA_CNPJ");
        builder.Property(x => x.TelefoneDDD).HasColumnName("TELEFONE_DDD");
        builder.Property(x => x.TelefoneNumero).HasColumnName("TELEFONE_NUMERO");
        builder.Property(x => x.CelularDDD).HasColumnName("CELULAR_DDD");
        builder.Property(x => x.CelularNumero).HasColumnName("CELULAR_NUMERO");
        builder.Property(x => x.ContatoNome).HasColumnName("CONTATO_NOME");
        builder.Property(x => x.ContatoEmail).HasColumnName("CONTATO_EMAIL");
        builder.Property(x => x.FretePorConta).HasColumnName("FRETE_POR_CONTA");
        builder.Property(x => x.Sincronizado).HasColumnName("SINCRONIZADO");
        builder.Property(x => x.EnderecoCobrancaIgualPrincipal).HasColumnName("ENDERECO_COBRANCA_IGUAL_PRINC");
        builder.Property(x => x.EnderecoEntregaIgualPrincipal).HasColumnName("ENDERECO_ENTREGA_IGUAL_PRINC");
    }
}