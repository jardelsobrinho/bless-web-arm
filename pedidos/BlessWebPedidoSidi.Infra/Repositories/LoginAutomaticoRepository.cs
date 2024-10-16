using BlessWebPedidoSidi.Domain.ControleSistema;
using BlessWebPedidoSidi.Domain.LoginAutomatico;
using BlessWebPedidoSidi.Infra.Context;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class LoginAutomaticoRepository(SidiDbContext context) : Repository<LoginAutomaticoEntity>(context), ILoginAutomaticoRepository;