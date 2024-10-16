using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Repositories;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using BlessWebPedidoSidi.Infra.Context;
using BlessWebPedidoSidi.Infra.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlessWebPedidoSidi.Infra.Repositories;

public class OrcamentoWebRepository(SidiDbContext context) : Repository<OrcamentoWebEntity>(context), IOrcamentoWebRepository
{
    public async Task<OrcamentoWebEntity?> CarregarDadosAsync(Expression<Func<OrcamentoWebEntity, bool>> expression)
    {
        return await _context.OrcamentosWeb
           .Where(expression)
           .Include(x => x.Itens)
           .ThenInclude(x => x.Grade)
           .SingleOrDefaultAsync();
    }

    public async Task<OrcamentoWebEntity> DuplicaAsync(string novoUuid, string uuid, string representanteCnpj, int usuarioCodigo)
    {
        var consulta = _context.OrcamentosWeb
           .AsNoTracking()
           .Where(x => x.Uuid == uuid && x.RepresentanteCnpj == representanteCnpj && x.UsuarioCodigo == usuarioCodigo)
           .Include(x => x.Itens)
           .ThenInclude(x => x.Grade);

        var orcamento = await consulta.FirstOrDefaultAsync() ??
            throw new EntidadeNaoEncontradaException("OWR01 - Orçamento não encontrado");

        orcamento.Uuid = novoUuid;
        orcamento.DataCriacao = DateTime.Now;
        orcamento.Status = EOrcamentoStatus.Aberto;
        orcamento.Id = 0;
        foreach (var item in orcamento.Itens)
        {
            item.Id = 0;
            foreach (var grade in item.Grade)
            {
                grade.Id = 0;
            }
        }
        _context.OrcamentosWeb.Add(orcamento);
        _context.SaveChanges();
        return orcamento;
    }
}