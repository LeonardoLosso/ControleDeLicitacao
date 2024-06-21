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

    public override void Editar(Item item)
    {
        RemoverAssociacoes(item.ID, item.EhCesta);

        base.Editar(item);
    }

    private void RemoverAssociacoes(int id, bool ehCesta)
    {
        if (ehCesta)
        {
            var itensAssociativos = _dbSetItens.Where(n => n.ItemPaiID == id).ToList();
            _dbSetItens.RemoveRange(itensAssociativos);
        }

        var nomesAssociativos = _dbSetNomes.Where(n => n.ItemID == id).ToList();
        _dbSetNomes.RemoveRange(nomesAssociativos);

        _context.SaveChanges();
    }
}
