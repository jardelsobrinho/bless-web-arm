using BlessWebPedidoSidi.Domain.Modelos.Entities;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class MarcaRepository(SidiDbContext context) : Repository<MarcaEntity>(context), IMarcaRepository
{
    public async Task<string> RetornaNomeMarca(int codigo)
    {
        var marcaEntity = await _context.Marcas
            .Where(x => x.Codigo == codigo)
            .SingleOrDefaultAsync();

        if (marcaEntity != null)
        {
            return marcaEntity.Nome;
        }
        return "";
    }
}
