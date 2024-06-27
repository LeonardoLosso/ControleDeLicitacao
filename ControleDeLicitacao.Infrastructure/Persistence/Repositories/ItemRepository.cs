using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Domain.ValueObjects;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class ItemRepository : Repository<Item>
{
    private readonly ItemContext _context;
    private readonly DbSet<ItemNomeAssociativo> _dbSetNomes;
    private readonly DbSet<ItemAssociativo> _dbSetItens;
    public ItemRepository(ItemContext context) : base(context)
    {
        _context = context;
        _dbSetNomes = _context.Set<ItemNomeAssociativo>();
        _dbSetItens = _context.Set<ItemAssociativo>();
    }

    public override async Task Editar(Item item)
    {
        await RemoverAssociacoes(item.ID, item.EhCesta);

        await base.Editar(item);
    }

    private async Task RemoverAssociacoes(int id, bool ehCesta)
    {
        if (ehCesta)
        {
            var itensAssociativos = await _dbSetItens.Where(n => n.ItemPaiID == id).ToListAsync();
            _dbSetItens.RemoveRange(itensAssociativos);
        }

        var nomesAssociativos = await _dbSetNomes.Where(n => n.ItemID == id).ToListAsync();
        _dbSetNomes.RemoveRange(nomesAssociativos);

        await _context.SaveChangesAsync();
    }
}
