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

    public virtual async Task<TEntity> Adicionar(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> ObterPorID(int id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.ID == id);
    }

    /* ID PERSONALIZADO
        public int GerarIdPersonalizado(string tipoCadastro)
        {
           // Lógica para gerar o ID personalizado
           // Por exemplo, buscar o maior ID existente para o tipo de cadastro e incrementar
           var maxId = Entidades
               .Where(e => e.TipoCadastro == tipoCadastro)
               .Select(e => e.Id)
               .DefaultIfEmpty(0)
               .Max();

           return maxId + 1;
       }
       public void InserirEntidadeComIdPersonalizado(Entidade entidade)
       {
           using (var transaction = Database.BeginTransaction())
           {
               try
               {
                   // Gerar ID personalizado baseado no tipo de cadastro
                   entidade.Id = GerarIdPersonalizado(entidade.TipoCadastro);

                   // Habilitar IDENTITY_INSERT para a tabela 'Entidades'
                   Database.ExecuteSqlRaw("SET IDENTITY_INSERT Entidades ON");

                   // Inserir a entidade com o ID personalizado
                   Entidades.Add(entidade);
                   SaveChanges();

                   // Desabilitar IDENTITY_INSERT para a tabela 'Entidades'
                   Database.ExecuteSqlRaw("SET IDENTITY_INSERT Entidades OFF");

                   // Commitar a transação
                   transaction.Commit();
               }
               catch (Exception)
               {
                   // Rollback em caso de erro
                   transaction.Rollback();
                   throw;
               }
           }
       }
    */
}
