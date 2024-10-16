using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using BlessWebPedidoSidi.Domain.Modelos.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.Usuario;
using Microsoft.EntityFrameworkCore;

namespace BlessWebPedidoSidi.Infra.Context;

public class SidiDbContext : DbContext
{
    public DbSet<ModeloEntity> Modelos => Set<ModeloEntity>();
    public DbSet<CorEntity> Cor => Set<CorEntity>();
    public DbSet<UsuarioEntity> Usuarios => Set<UsuarioEntity>();
    public DbSet<ControleSistemaEntity> ControleSistemas => Set<ControleSistemaEntity>();
    public DbSet<ClienteWebEntity> ClientesWeb => Set<ClienteWebEntity>();
    public DbSet<ClienteWebEnderecoEntity> ClientesWebEndereco => Set<ClienteWebEnderecoEntity>();
    public DbSet<OrcamentoWebEntity> OrcamentosWeb => Set<OrcamentoWebEntity>();
    public DbSet<OrcamentoWebItemEntity> OrcamentosWebItens => Set<OrcamentoWebItemEntity>();
    public DbSet<OrcamentoWebItemGradeEntity> OrcamentosWebItensGrade => Set<OrcamentoWebItemGradeEntity>();
    public DbSet<LoginAutomaticoEntity> LoginAutomatico => Set<LoginAutomaticoEntity>();
    public DbSet<CidadeEntity> Cidades => Set<CidadeEntity>();
    public DbSet<MarcaEntity> Marcas => Set<MarcaEntity>();

    public SidiDbContext() { }
    public SidiDbContext(DbContextOptions<SidiDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SidiDbContext).Assembly);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseFirebird(Conexao.RetornaString());
        }
    }
}
