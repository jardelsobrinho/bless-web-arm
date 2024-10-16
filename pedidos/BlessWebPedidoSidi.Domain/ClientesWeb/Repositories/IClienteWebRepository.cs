using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.Shared;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Domain.ClientesWeb.Repositories;

public interface IClienteWebRepository : IRepository<ClienteWebEntity>
{
    Task<ClienteWebEntity> CarregarDadosAsync(Expression<Func<ClienteWebEntity, bool>> expression);
}