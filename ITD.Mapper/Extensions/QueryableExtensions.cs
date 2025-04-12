using System.Linq.Expressions;

namespace ITD.Mapper.Extensions;
public static class QueryableExtensions
{
    public static IQueryable<TDestination> ProjectTo<TDestination, TSource>(this IQueryable<TSource> source)
    {
        Expression<Func<TSource, TDestination>> expr = Mapper.GetProjectionExpression<TSource, TDestination>();
        return source.Select(expr);
    }
}
