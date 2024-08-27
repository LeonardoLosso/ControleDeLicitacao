using ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;
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
    private readonly DbSet<EmpenhoPolicia> _dbSetEmpenhoPolicia;

    private readonly DbSet<Nota> _dbSetNota;

    private readonly DbSet<Reajuste> _dbSetReajuste;

    public BaixaRepository(BaixaContext context) : base(context)
    {
        _context = context;
        _dbSetItens = _context.Set<ItemDeBaixa>();

        _dbSetEmpenho = _context.Set<Empenho>();
        _dbSetEmpenhoPolicia = _context.Set<EmpenhoPolicia>();
        _dbSetNota = _context.Set<Nota>();


        _dbSetReajuste = _context.Set<Reajuste>();
    }

    public async Task<BaixaLicitacao?> ObterBaixaCompletaPorID(int id)
    {
        return await Buscar()
            .AsNoTracking()
            .Where(w => w.ID == id)
                .Include(w => w.Itens.OrderBy(i => i.Nome))
                .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    public override async Task Editar(BaixaLicitacao updatedBaixa)
    {
        var existingBaixa = await _context.Set<BaixaLicitacao>()
            .Include(a => a.Itens)
            .FirstOrDefaultAsync(a => a.ID == updatedBaixa.ID);

        if (existingBaixa is null)
        {
            throw new InvalidOperationException("BaixaLicitacao not found.");
        }

        _context.Entry(existingBaixa).CurrentValues.SetValues(updatedBaixa);

        foreach (var existingItem in existingBaixa.Itens.ToList())
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
            if (!existingBaixa.Itens
                .Any(i => i.BaixaID == newItem.BaixaID && i.ID == newItem.ID && i.ValorUnitario == newItem.ValorUnitario))
            {
                existingBaixa.Itens.Add(newItem);
            }
        }

        await _context.SaveChangesAsync();
    }
    public async Task AdicionarEmpenho(Empenho entity, bool atualizaBaixa = false)
    {
        _dbSetEmpenho.Add(entity);
        await _context.SaveChangesAsync();

        if (atualizaBaixa)
            await AtualizarBaixa(entity.BaixaID);

    }
    public async Task AdicionarNota(Nota entity)
    {
        _dbSetNota.Add(entity);
        await _context.SaveChangesAsync();


        await AtualizarEmpenho(entity.EmpenhoID);
    }
    public async Task ExcluirEmpenho(Empenho entity)
    {
        var baixaID = entity.BaixaID;
        _dbSetEmpenho.Remove(entity);
        await AtualizarBaixa(baixaID);
        await _context.SaveChangesAsync();
    }
    public async Task ExcluirNota(Nota entity)
    {
        _dbSetNota.Remove(entity);
        await _context.SaveChangesAsync();

        await AtualizarEmpenho(entity.EmpenhoID);
    }
    public IQueryable<Empenho> BuscarEmpenho()
    {
        return _dbSetEmpenho.AsQueryable();
    }
    public IQueryable<EmpenhoPolicia> BuscarEmpenhoPolicia()
    {
        return _dbSetEmpenhoPolicia.AsQueryable();
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
    public async Task EditarNota(Nota updatedNota)
    {
        var existingNota = await _dbSetNota
            .Include(a => a.Itens)
            .FirstOrDefaultAsync(a => a.ID == updatedNota.ID);

        if (existingNota is null)
        {
            throw new InvalidOperationException("Nota not found.");
        }

        _context.Entry(existingNota).CurrentValues.SetValues(updatedNota);

        foreach (var existingItem in existingNota.Itens.ToList())
        {
            var updatedItem = updatedNota.Itens
                                        .FirstOrDefault(
                                            i => i.NotaID == existingItem.NotaID
                                            && i.ID == existingItem.ID);

            if (updatedItem is null)
                _context.ItemDeNota.Remove(existingItem);
            else
                _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
        }

        foreach (var newItem in updatedNota.Itens)
        {
            if (!existingNota.Itens
                .Any(i => i.NotaID == newItem.NotaID && i.ID == newItem.ID))
            {
                existingNota.Itens.Add(newItem);
            }
        }

        await _context.SaveChangesAsync();

        await AtualizarEmpenho(updatedNota.EmpenhoID);
    }

    //--------------------------------------------------------
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

    //---------------------[PRIVATE]--------------------------
    private async Task<List<Empenho>> ObterEmpenhosPorBaixa(int id)
    {
        return await BuscarEmpenho()
            .Where(e => e.BaixaID == id)
            .Include(i => i.Itens)
            .ToListAsync();
    }
    private async Task AtualizarBaixa(int id)
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

                if (itemEmpenho is null) continue;

                valorEntregue += itemEmpenho.ValorEntregue;
                qtdeEmpenhada += itemEmpenho.QtdeEmpenhada;
            }
            item.QtdeEmpenhada = qtdeEmpenhada;
            item.ValorEmpenhado = qtdeEmpenhada * item.ValorUnitario;
            item.QtdeAEmpenhar = item.QtdeLicitada - item.QtdeEmpenhada;
            item.Saldo = item.ValorLicitado - item.ValorEmpenhado;
        }

        _context.Update(baixa);
        await _context.SaveChangesAsync();
    }
    private async Task AtualizarEmpenho(int empenhoID)
    {
        var empenho = await BuscarEmpenhoPorID(empenhoID);

        if (empenho is null) return;

        var notas = await BuscarNota()
            .Where(n => n.EmpenhoID == empenhoID)
            .Include(i => i.Itens)
            .ToListAsync();

        if (!notas.Any()) return;

        foreach (var item in empenho.Itens)
        {
            double quantidade = 0;

            foreach (var nota in notas)
            {
                var itemNota = nota.Itens
                    .FirstOrDefault(
                        x => x.ID == item.ID &&
                        x.ValorUnitario == item.ValorUnitario);

                if (itemNota is null) continue;

                quantidade += itemNota.Quantidade;
            }

            item.QtdeEntregue = quantidade;
            item.QtdeAEntregar = item.QtdeEmpenhada - item.QtdeEntregue;
            item.ValorEntregue = item.QtdeEntregue * item.ValorUnitario;
        }

        empenho.Saldo = empenho.Valor - empenho.Itens.Sum(i => i.ValorEntregue);

        _context.Update(empenho);
        await _context.SaveChangesAsync();

        await AtualizarBaixa(empenho.BaixaID);
    }

    public async Task SalvarBaixaPolicia(List<EmpenhoPolicia> empenhos, int id)
    {
        var remover = await BuscarEmpenhoPolicia()
            .AsNoTracking()
            .Where(e => e.BaixaID == id)
            .ToListAsync();

        _dbSetEmpenhoPolicia.RemoveRange(remover);

        _dbSetEmpenhoPolicia.AddRange(empenhos);
        await _context.SaveChangesAsync();
    }
}