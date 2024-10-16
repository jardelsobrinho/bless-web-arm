using BlessWebPedidoSidi.Domain.Usuario;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class UsuarioRepository(SidiDbContext context) : Repository<UsuarioEntity>(context), IUsuarioRepository
{
    public async Task<UsuarioEntity?> CarregaDadosAsync(Expression<Func<UsuarioEntity, bool>> expression)
    {
        var consulta = await _context.Usuarios
            .Where(expression)
            .SingleOrDefaultAsync();
        return consulta;
    }
}