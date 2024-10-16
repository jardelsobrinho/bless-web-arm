using BlessWebPedidoSidi.Domain.Shared;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Domain.Usuario;

public interface IUsuarioRepository: IRepository<UsuarioEntity>
{
    Task<UsuarioEntity?> CarregaDadosAsync(Expression<Func<UsuarioEntity, bool>> expression);
}