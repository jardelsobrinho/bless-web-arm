using BlessWebPedidoSidi.Domain.Cidades;
using BlessWebPedidoSidi.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class CidadeRepository(SidiDbContext context) : Repository<CidadeEntity>(context), ICidadeRepository
{
    public async Task<int> RetornaCodigoClienteAsync(int codigoIbge, string nomeCidade)
    {
        var consulta = _context.Cidades
            .Where(x => x.CodigoIbge == codigoIbge);

        var listaCidades = await consulta.ToListAsync();

        if (listaCidades.Count == 0)
            return 0;

        if (listaCidades.Count == 1) 
            return listaCidades.First().Codigo;

        var consultaPorNome = _context.Cidades
            .Where(x => x.CodigoIbge == codigoIbge && x.Nome.ToUpper() == nomeCidade.ToUpper());

        var listaCidadesPorNome = await consultaPorNome.ToListAsync();
        if (listaCidadesPorNome.Count == 0 && listaCidades.Count > 0)
        {
            return listaCidades.First().Codigo;
        }
        else if (listaCidadesPorNome.Count > 0)
        {
            return listaCidadesPorNome.First().Codigo;
        }
        return 0;
    }
}
