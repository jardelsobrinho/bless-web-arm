using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class OrcamentoWebItemRepository(SidiDbContext context) : Repository<OrcamentoWebItemEntity>(context), IOrcamentoWebItemRepository
{
    public async Task<OrcamentoWebItemEntity?> CarregarDadosAsync(Expression<Func<OrcamentoWebItemEntity, bool>> expression)
    {
        return await _context.OrcamentosWebItens
           .Where(expression)
           .Include(x => x.Grade)
           .SingleOrDefaultAsync();
    }
}