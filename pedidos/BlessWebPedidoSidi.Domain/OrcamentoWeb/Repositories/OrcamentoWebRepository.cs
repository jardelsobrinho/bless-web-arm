using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.Shared;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;

public interface IOrcamentoWebRepository : IRepository<OrcamentoWebEntity>
{
    Task<OrcamentoWebEntity?> CarregarDadosAsync(Expression<Func<OrcamentoWebEntity, bool>> expression);
    Task<OrcamentoWebEntity> DuplicaAsync(string novoUuid, string uuid, string representanteCnpj, int usuarioCodigo);
}