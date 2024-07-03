namespace ControleDeLicitacao.Infrastructure.Persistence.Interface;

public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    Task<TEntity> ObterPorID(int id);
    Task <TEntity>Adicionar(TEntity entity);
    Task Editar(TEntity entity);
    IQueryable<TEntity> Buscar();
}
