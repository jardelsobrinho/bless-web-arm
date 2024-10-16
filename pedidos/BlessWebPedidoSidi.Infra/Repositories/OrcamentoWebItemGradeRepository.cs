using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;
using BlessWebPedidoSidi.Infra.Context;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class OrcamentoWebItemGradeRepository(SidiDbContext context) : Repository<OrcamentoWebItemGradeEntity>(context), IOrcamentoWebItemGradeRepository;