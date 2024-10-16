using BlessWebPedidoSidi.Domain.Modelos.Entities;
using BlessWebPedidoSidi.Domain.Modelos.Repositories;
using BlessWebPedidoSidi.Infra.Context;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class CorRepository(SidiDbContext context) : Repository<CorEntity>(context), ICorRepository;
