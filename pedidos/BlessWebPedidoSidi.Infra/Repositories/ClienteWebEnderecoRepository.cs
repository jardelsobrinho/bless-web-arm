using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.ClientesWeb.Repositories;
using BlessWebPedidoSidi.Infra.Context;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class ClienteWebEnderecoRepository(SidiDbContext context) : Repository<ClienteWebEnderecoEntity>(context), IClienteWebEnderecoRepository;