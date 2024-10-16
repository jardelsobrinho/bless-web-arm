using BlessWebPedidoSidi.Domain.Shared;
using BlessWebPedidoSidi.Domain.Modelos.Entities;
using System.Linq.Expressions;


namespace BlessWebPedidoSidi.Domain.Modelos.Repositories;

public interface IModeloRepository : IRepository<ModeloEntity>
{
    Task<IList<ModeloEntity>> CarregarDadosAsync(Expression<Func<ModeloEntity, bool>> expression);
    
}
