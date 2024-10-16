using BlessWebPedidoSidi.Domain.Modelos.Entities;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class ModeloRepository(SidiDbContext context) : Repository<ModeloEntity>(context), IModeloRepository
{
    public async Task<IList<ModeloEntity>>  CarregarDadosAsync(Expression<Func<ModeloEntity, bool>> expression)
    {
        var consulta = await _context.Modelos
            .Where(expression)
            .ToListAsync();
        return consulta;
    }
}
