using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class BaixaRepository : Repository<BaixaLicitacao>
{
    private readonly BaixaContext _context;
    private readonly DbSet<ItemDeBaixa> _dbSetItens;

    private readonly DbSet<Empenho> _dbSetEmpenho;

    private readonly DbSet<Nota> _dbSetNota;

    public BaixaRepository(BaixaContext context) : base(context)
    {
        _context = context;
        _dbSetEmpenho = _context.Set<Empenho>();
        _dbSetItens = _context.Set<ItemDeBaixa>();
    }

    public async Task<BaixaLicitacao?> ObterBaixaCompletaPorID(int id)
    {
        return await Buscar()
            .AsNoTracking()
            .Where(w => w.ID == id)
                .Include(w => w.Itens)
                .AsNoTracking()
            .FirstOrDefaultAsync();
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
    public async Task AdicionarEmpenho(Empenho entity)
    {
        _dbSetEmpenho.Add(entity);
        await _context.SaveChangesAsync();
    }
    public async Task AdicionarNota(Nota entity)
    {
        _dbSetNota.Add(entity);
        await _context.SaveChangesAsync();

        if(entity.Itens.Count > 0)
        {
            //await AtualizarEmpenho();
        }
    }
    public async Task ExcluirEmpenho(Empenho entity)
    {
        _dbSetEmpenho.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public IQueryable<Empenho> BuscarEmpenho()
    {
        return _dbSetEmpenho.AsQueryable();
    }
    public IQueryable<Nota> BuscarNota()
    {
        return _dbSetNota.AsQueryable();
    }
    public IQueryable<ItemDeBaixa> BuscarItens()
    {
        return _dbSetItens.AsQueryable();
    }
    public async Task<Empenho?> BuscarEmpenhoPorID(int id)
    {
        return await _dbSetEmpenho
            .AsNoTracking()
            .Include(i => i.Itens)
                .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ID == id);
    }
    public async Task<Nota?> BuscarNotaPorID(int id)
    {
        return await _dbSetNota
            .AsNoTracking()
            .Include(i => i.Itens)
                .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ID == id);
    }
    public async Task EditarEmpenho(Empenho updatedEmpenho)
    {
        var inativo = updatedEmpenho.Status == 2;

        var existingEmpenho = await _dbSetEmpenho
            .Include(a => a.Itens)
            .FirstOrDefaultAsync(a => a.ID == updatedEmpenho.ID);

        if (existingEmpenho is null)
        {
            throw new InvalidOperationException("Empenho not found.");
        }

        _context.Entry(existingEmpenho).CurrentValues.SetValues(updatedEmpenho);

        foreach (var existingItem in existingEmpenho.Itens.ToList())
        {
            var updatedItem = updatedEmpenho.Itens
                                        .FirstOrDefault(
                                            i => i.EmpenhoID == existingItem.EmpenhoID
                                            && i.ID == existingItem.ID
                                            && i.ValorUnitario == existingItem.ValorUnitario);

            if (updatedItem is null)
            {
                _context.ItemDeEmpenho.Remove(existingItem);
            }
            else
            {
                if (inativo)
                {
                    updatedItem.QtdeEmpenhada = updatedItem.QtdeEntregue;
                    updatedItem.QtdeAEntregar = 0;
                    updatedItem.Total = updatedItem.ValorEntregue;
                }
                _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
            }
        }

        foreach (var newItem in updatedEmpenho.Itens)
        {
            if (!existingEmpenho.Itens
                .Any(i => i.EmpenhoID == newItem.EmpenhoID && i.ID == newItem.ID && i.ValorUnitario == i.ValorUnitario))
            {
                existingEmpenho.Itens.Add(newItem);
            }
        }

        await _context.SaveChangesAsync();

        await AtualizarBaixa(updatedEmpenho.BaixaID);
    }
    public async Task AtualizarBaixa(int id)
    {
        var baixa = await ObterBaixaCompletaPorID(id);

        if (baixa is null) return;

        var empenhos = await ObterEmpenhosPorBaixa(id);

        if (empenhos is null) return;

        foreach (var item in baixa.Itens)
        {
            double valorEntregue = 0;
            double qtdeEmpenhada = 0;

            foreach (var empenho in empenhos)
            {
                var itemEmpenho = empenho.Itens
                .FirstOrDefault(
                    x => x.ID == item.ID &&
                    x.BaixaID == item.BaixaID &&
                    x.ValorUnitario == item.ValorUnitario &&
                    x.ItemDeBaixa == true);

                if (itemEmpenho is not null)
                {
                    valorEntregue += itemEmpenho.ValorEntregue;
                    qtdeEmpenhada += itemEmpenho.QtdeEmpenhada;
                }
            }
            item.QtdeEmpenhada = qtdeEmpenhada;
            item.ValorEmpenhado = qtdeEmpenhada * item.ValorUnitario;
            item.QtdeAEmpenhar = item.QtdeLicitada - item.QtdeEmpenhada;
            item.Saldo = item.ValorLicitado - item.ValorEmpenhado;
        }

        _context.Update(baixa);
        _context.SaveChanges();
    }
    public async Task<List<Empenho>> ObterEmpenhosPorBaixa(int id)
    {
        return await BuscarEmpenho()
            .Where(e => e.BaixaID == id)
            .Include(i => i.Itens)
            .ToListAsync();
    }

}