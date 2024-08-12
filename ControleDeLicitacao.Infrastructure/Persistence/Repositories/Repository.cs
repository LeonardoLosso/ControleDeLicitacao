using ControleDeLicitacao.Domain.Iterfaces;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IDominio
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
    }

    public IQueryable<TEntity> Buscar()
    {
        return _dbSet.AsQueryable();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public virtual async Task Editar(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Adicionar(TEntity entity)
    {
        entity.Status = 1;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<TEntity> ObterPorID(int id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.ID == id);
    }

}
