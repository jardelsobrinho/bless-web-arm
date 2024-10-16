using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Domain.ClientesWeb.Repositories;
using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;
using BlessWebPedidoSidi.Domain.Usuario;

namespace BlessWebPedidoSidi.Domain.Shared
{
    public interface IUnitOfWork : IDisposable
    {
        IModeloRepository ModeloRepository { get; }
        ICorRepository CorRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        IControleSistemaRepository ControleSistemaRepository { get; }
        IClienteWebRepository ClienteWebRepository { get; }
        IClienteWebEnderecoRepository ClienteWebEnderecoRepository { get; }
        ICidadeRepository CidadeRepository { get; }
        IOrcamentoWebRepository OrcamentoWebRepository { get; }
        IOrcamentoWebItemRepository OrcamentoWebItemRepository { get; }
        IOrcamentoWebItemGradeRepository OrcamentoWebItemGradeRepository { get; }
        ILoginAutomaticoRepository LoginAutomaticoRepository { get; }

        Task StartTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
