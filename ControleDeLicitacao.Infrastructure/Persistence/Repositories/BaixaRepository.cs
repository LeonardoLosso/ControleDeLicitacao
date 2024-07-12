using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class BaixaRepository : Repository<BaixaLicitacao>
{
    private readonly BaixaContext _context;

    public BaixaRepository(BaixaContext context) : base(context)
    {
        _context = context;
    }

    public override async Task Adicionar(BaixaLicitacao entity)
    {

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [BaixaLicitacao] ON");

                _context.BaixaLicitacao.Add(entity);
                _context.SaveChanges();

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [BaixaLicitacao] OFF");

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public override async Task Editar(BaixaLicitacao updatedBaixa)
    {
        var existingAta = await _context.Set<BaixaLicitacao>()
            .Include(a => a.Itens)
            .FirstOrDefaultAsync(a => a.ID == updatedBaixa.ID);

        if (existingAta is null)
        {
            throw new InvalidOperationException("BaixaLicitacao not found.");
        }

        _context.Entry(existingAta).CurrentValues.SetValues(updatedBaixa);

        foreach (var existingItem in existingAta.Itens.ToList())
        {
            var updatedItem = updatedBaixa.Itens
                                        .FirstOrDefault(
                                            i => i.BaixaID == existingItem.BaixaID 
                                            && i.ID == existingItem.ID
                                            && i.ValorUnitario == existingItem.ValorUnitario);

            if (updatedItem is null)
            {
                _context.ItemDeBaixa.Remove(existingItem);
            }
            else
            {
                _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
            }
        }

        foreach (var newItem in updatedBaixa.Itens)
        {
            if (!existingAta.Itens
                .Any(i => i.BaixaID == newItem.BaixaID && i.ID == newItem.ID && i.ValorUnitario == i.ValorUnitario))
            {
                existingAta.Itens.Add(newItem);
            }
        }

        await _context.SaveChangesAsync();

    }
}