using BlessWebPedidoSidi.Domain.Ufs;
using BlessWebPedidoSidi.Infra.Context;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class UfRepository(SidiDbContext context) : Repository<UfEntity>(context), IUfRepository;