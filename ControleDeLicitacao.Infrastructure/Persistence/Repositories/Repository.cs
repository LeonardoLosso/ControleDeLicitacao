using ControleDeLicitacao.Domain.Entities.Log;
using ControleDeLicitacao.Domain.Iterfaces;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IDominio
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly LogContext _logContext;
    private readonly UserKeeper _userKeeper;

    public Repository(DbContext context, LogContext logContext, UserKeeper userKeeper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();

        _logContext = logContext;
        _userKeeper = userKeeper;
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

    }

    public virtual async Task Adicionar(TEntity entity)
    {
        entity.Status = 1;
        await _dbSet.AddAsync(entity);
        await SalvaContexto();
    }
    public async Task SalvaContexto()
    {
        try
        {
            await SaveLogAsync();

        }
        finally
        {

            await _context.SaveChangesAsync();
        }
    }
    public async Task<TEntity> ObterPorID(int id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.ID == id);
    }


    //-------------------------------------------------------------------------

    public async Task SaveLogAsync(string modulo = "", CancellationToken cancellationToken = default)
    {
        var auditLogs = new List<LogEntity>();
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            var auditLog = new LogEntity
            {
                Id = 0,
                Path = entry.Entity.GetType().Name,
                RecordId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue as int?,
                UserName = _userKeeper.CurrentUser,
                Horario = DateTime.Now
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditLog.Operacao = "Insert";
                    auditLog.NewValue = JsonConvert.SerializeObject(entry.CurrentValues.ToObject());
                    break;
                case EntityState.Modified:
                    auditLog.Operacao = "Update";
                    auditLog.OldValue = JsonConvert.SerializeObject(entry.OriginalValues.ToObject());
                    auditLog.NewValue = JsonConvert.SerializeObject(entry.CurrentValues.ToObject());
                    break;
                case EntityState.Deleted:
                    auditLog.Operacao = "Delete";
                    auditLog.OldValue = JsonConvert.SerializeObject(entry.OriginalValues.ToObject());
                    break;
            }

            auditLogs.Add(auditLog);
        }

        // Salva os registros de auditoria no banco
        await _logContext.AddRangeAsync(auditLogs, cancellationToken);

        await _logContext.SaveChangesAsync(cancellationToken);
    }

}
