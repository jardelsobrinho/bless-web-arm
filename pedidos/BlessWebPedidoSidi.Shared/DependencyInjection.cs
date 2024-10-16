using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Domain.ClientesWeb.Repositories;
using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;
using BlessWebPedidoSidi.Domain.Shared;
using BlessWebPedidoSidi.Domain.Ufs;
using BlessWebPedidoSidi.Domain.Usuario;
using BlessWebPedidoSidi.Infra.Context;
using BlessWebPedidoSidi.Infra.Repositories;
using BlessWebPedidoSidi.Application.Shared;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using Microsoft.Extensions.Configuration;

namespace BlessWebPedidoSidi.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, string[] args, IConfiguration configuration)
        {
            Conexao.ConexaoArguments = new ConexaoArguments(args);
            Conexao.Configuration = configuration;

            services.AddDbContext<SidiDbContext>();
            services.AddScoped<IDbConnection>((sp) => new FbConnection(Conexao.RetornaString()));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IModeloRepository, ModeloRepository>();
            services.AddScoped<ICorRepository, CorRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IControleSistemaRepository, ControleSistemaRepository>();
            services.AddScoped<IClienteWebRepository, ClienteWebRepository>();
            services.AddScoped<IClienteWebEnderecoRepository, ClienteWebEnderecoRepository>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();
            services.AddScoped<IOrcamentoWebRepository, OrcamentoWebRepository>();
            services.AddScoped<IOrcamentoWebItemRepository, OrcamentoWebItemRepository>();
            services.AddScoped<IOrcamentoWebItemGradeRepository, OrcamentoWebItemGradeRepository>();
            services.AddScoped<IUfRepository, UfRepository>();
            services.AddScoped<ILoginAutomaticoRepository, LoginAutomaticoRepository>();
            services.AddScoped<IMarcaRepository, MarcaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriptografia, Criptografia>();
            return services;
        }
    }

}
