using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.Shared;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;

public interface IOrcamentoWebItemRepository : IRepository<OrcamentoWebItemEntity>
{
    Task<OrcamentoWebItemEntity?> CarregarDadosAsync(Expression<Func<OrcamentoWebItemEntity, bool>> expression);
}