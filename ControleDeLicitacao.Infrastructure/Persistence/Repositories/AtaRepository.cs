using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class AtaRepository : Repository<AtaLicitacao>
{
    private readonly AtaContext _context;

    public AtaRepository(AtaContext context) : base(context)
    {
        _context = context;
    }

    public async Task Editar(AtaLicitacao updatedAta)
    {
        var existingAta = await _context.Set<AtaLicitacao>()
            .Include(a => a.Itens)
            .FirstOrDefaultAsync(a => a.ID == updatedAta.ID);
        if (existingAta == null)
        {
            throw new InvalidOperationException("AtaLicitacao not found.");
        }
        _context.Entry(existingAta).CurrentValues.SetValues(updatedAta);
        foreach (var existingItem in existingAta.Itens.ToList())
        {
            var updatedItem = updatedAta.Itens
                                        .FirstOrDefault(i => i.AtaID == existingItem.AtaID && i.ID == existingItem.ID);

            if (updatedItem == null)
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
            if (!existingAta.Itens.Any(i => i.AtaID == newItem.AtaID && i.ID == newItem.ID))
            {
                existingAta.Itens.Add(newItem);
            }
        }

        await _context.SaveChangesAsync();
    }
}
