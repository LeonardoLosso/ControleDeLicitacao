namespace ControleDeLicitacao.Infrastructure.Persistence.Interface;

public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    TEntity ObterPorID(int id);
    void Adicionar(TEntity entity);
    void Editar(TEntity entity);
    IQueryable<TEntity> Buscar();
}
