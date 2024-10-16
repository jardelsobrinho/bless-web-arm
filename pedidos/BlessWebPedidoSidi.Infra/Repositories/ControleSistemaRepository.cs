using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class ControleSistemaRepository(SidiDbContext context) : Repository<ControleSistemaEntity>(context), IControleSistemaRepository
{
    public async Task<ControleSistemaEntity> RetornaAsync()
    {
        return await _context.ControleSistemas.FirstAsync();
    }
}