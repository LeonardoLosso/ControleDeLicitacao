using System.Linq.Expressions;

namespace ControleDeLicitacao.Common;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> BuscarPalavraChave<TEntity>(this IQueryable<TEntity> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        search = search.Trim();

        var properties = typeof(TEntity).GetProperties()
                .Where(p => p.PropertyType == typeof(string)).ToList();

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression searchExpression = null;

        foreach (var property in properties)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var searchValue = Expression.Constant(search);
            var containsExpression = Expression.Call(propertyAccess, containsMethod, searchValue);

            if (searchExpression is null)
            {
                searchExpression = containsExpression;
            }
            else
            {
                searchExpression = Expression.OrElse(searchExpression, containsExpression);
            }
        }

        if (searchExpression != null)
        {
            var lambda = Expression.Lambda<Func<TEntity, bool>>(searchExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }
}
