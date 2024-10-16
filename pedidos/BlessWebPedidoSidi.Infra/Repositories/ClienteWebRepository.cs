using BlessWebPedidoSidi.Domain.ClientesWeb.Entities;
using BlessWebPedidoSidi.Domain.ClientesWeb.Repositories;
using BlessWebPedidoSidi.Infra.Context;
using BlessWebPedidoSidi.Infra.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class ClienteWebRepository(SidiDbContext context) : Repository<ClienteWebEntity>(context), IClienteWebRepository
{
    public async Task<ClienteWebEntity> CarregarDadosAsync(Expression<Func<ClienteWebEntity, bool>> expression)
    {
        var consulta = await _context.ClientesWeb
            .Include(x => x.Enderecos)
            .Where(expression).FirstOrDefaultAsync();

        return consulta ?? throw new EntidadeNaoEncontradaException("CR01 - Cliente não encontrado");
    }
}