using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlessWebPedidoSidi.Infra.Maps;

public class OrcamentoWebMap : IEntityTypeConfiguration<OrcamentoWebEntity>
{
    public void Configure(EntityTypeBuilder<OrcamentoWebEntity> builder)
    {
        builder.ToTable("WEB_ORCAMENTO");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Uuid).HasColumnName("UUID");
        builder.Property(x => x.DataCriacao).HasColumnName("DATA_CRIACAO");
        builder.Property(x => x.DataEmissao).HasColumnName("DATA_EMISSAO");
        builder.Property(x => x.DataEntrega).HasColumnName("DATA_ENTREGA");
        builder.Property(x => x.ValorTotal).HasColumnName("VALOR_TOTAL");
        builder.Property(x => x.ClienteCnpjCpf).HasColumnName("CLIENTE_CNPJ_CPF");
        builder.Property(x => x.TabelaPrecoCodigo).HasColumnName("TABELA_PRECO_CODIGO");
        builder.Property(x => x.CondicaoPagamentoCodigo).HasColumnName("CONDICAO_PAGTO_CODIGO");
        builder.Property(x => x.TipoEstoqueCodigo).HasColumnName("TIPO_ESTOQUE_CODIGO");
        builder.Property(x => x.RepresentanteCnpj).HasColumnName("REPRESENTANTE_CNPJ");
        builder.Property(x => x.EmpresaCnpj).HasColumnName("EMPRESA_CNPJ");
        builder.Property(x => x.UsuarioCodigo).HasColumnName("USUARIO_CODIGO");
        builder.Property(x => x.Frete).HasColumnName("FRETE");
        builder.Property(x => x.ClienteNome).HasColumnName("CLIENTE_NOME");
        builder.Property(x => x.Obs).HasColumnName("OBS");
        builder.Property(x => x.Status).HasColumnName("STATUS")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString(),
                v => (EOrcamentoStatus)Enum.Parse(typeof(EOrcamentoStatus), v));
    }
}