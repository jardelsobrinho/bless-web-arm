using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Domain.ClientesWeb.Repositories;
using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;
using BlessWebPedidoSidi.Domain.Shared;
using BlessWebPedidoSidi.Domain.Usuario;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class UnitOfWork(SidiDbContext context, IModeloRepository modeloRepository,
    IUsuarioRepository usuarioRepository, IControleSistemaRepository controleSistemaRepository,
    IClienteWebRepository clienteWebRepository, IClienteWebEnderecoRepository clienteWebEnderecoRepository,
    ICidadeRepository cidadeRepository, IOrcamentoWebRepository orcamentoWebRepository,
    IOrcamentoWebItemRepository orcamentoWebItemRepository,
    IOrcamentoWebItemGradeRepository orcamentoWebItemGradeRepository,
    ICorRepository corRepository,
    ILoginAutomaticoRepository loginAutomaticoRepository) : IUnitOfWork
{
    public IModeloRepository ModeloRepository { get; } = modeloRepository;
    public ICorRepository CorRepository { get; } = corRepository;
    public IUsuarioRepository UsuarioRepository { get; } = usuarioRepository;
    public IControleSistemaRepository ControleSistemaRepository { get; } = controleSistemaRepository;
    public IClienteWebRepository ClienteWebRepository { get; } = clienteWebRepository;
    public IClienteWebEnderecoRepository ClienteWebEnderecoRepository { get; } = clienteWebEnderecoRepository;
    public ICidadeRepository CidadeRepository { get; } = cidadeRepository;
    public IOrcamentoWebRepository OrcamentoWebRepository { get; } = orcamentoWebRepository;
    public IOrcamentoWebItemRepository OrcamentoWebItemRepository { get; } = orcamentoWebItemRepository;
    public IOrcamentoWebItemGradeRepository OrcamentoWebItemGradeRepository { get; } = orcamentoWebItemGradeRepository;
    public ILoginAutomaticoRepository LoginAutomaticoRepository { get; } = loginAutomaticoRepository;

    private readonly SidiDbContext _context = context;
    private IDbContextTransaction? _transaction;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async Task StartTransactionAsync()
    {
        if (_transaction != null)
            throw new Exception("UOW01 - Já existe uma transação iniciada");

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _transaction!.CommitAsync();
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction!.RollbackAsync();
        _transaction.Dispose();
        _transaction = null;
    }
}
