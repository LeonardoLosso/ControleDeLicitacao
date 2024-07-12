using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class AtaRepository : Repository<AtaLicitacao>
{
    private readonly AtaContext _context;
    private readonly DbSet<Reajuste> _dbSetReajuste;

    public AtaRepository(AtaContext context) : base(context)
    {
        _context = context;
        _dbSetReajuste = _context.Set<Reajuste>();
    }

    public async Task Editar(AtaLicitacao updatedAta)
    {
        var existingAta = await _context.Set<AtaLicitacao>()
            .Include(a => a.Itens)
            .FirstOrDefaultAsync(a => a.ID == updatedAta.ID);
        if (existingAta is null)
        {
            throw new InvalidOperationException("AtaLicitacao not found.");
        }
        _context.Entry(existingAta).CurrentValues.SetValues(updatedAta);
        foreach (var existingItem in existingAta.Itens.ToList())
        {
            var updatedItem = updatedAta.Itens
                                        .FirstOrDefault(
                                            i => i.AtaID == existingItem.AtaID 
                                            && i.ID == existingItem.ID 
                                            && i.ValorUnitario == existingItem.ValorUnitario);

            if (updatedItem is null)
            {
                _context.ItemDeAta.Remove(existingItem);
            }
            else
            {
                _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
            }
        }

        foreach (var newItem in updatedAta.Itens)
        {
            if (!existingAta.Itens
                .Any(i => i.AtaID == newItem.AtaID && i.ID == newItem.ID && i.ValorUnitario == newItem.ValorUnitario))
            {
                existingAta.Itens.Add(newItem);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task AdicionarReajuste(Reajuste reajuste)
    {
        _dbSetReajuste.Add(reajuste);
        await _context.SaveChangesAsync();
    }
    public IQueryable<Reajuste> BuscarReajuste()
    {
        return _dbSetReajuste.AsQueryable();
    }

    public async Task ExcluirReajuste(Reajuste reajuste)
    {
        _dbSetReajuste.Remove(reajuste);
        await _context.SaveChangesAsync();
    }
}
