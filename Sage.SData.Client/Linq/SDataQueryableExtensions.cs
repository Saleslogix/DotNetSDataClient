using System;
using System.Linq;
using System.Linq.Expressions;

namespace Sage.SData.Client.Linq
{
    public static class SDataQueryableExtensions
    {
        public static IQueryable<T> WithPrecedence<T>(this IQueryable<T> query, int value)
        {
            return new SDataQueryable<T>(
                query.Provider,
                Expression.Call(
                    new Func<IQueryable<T>, int, IQueryable<T>>(WithPrecedence).Method,
                    query.Expression,
                    Expression.Constant(value)));
        }

        public static IQueryable<TSource> Fetch<TSource, TRelated>(this IQueryable<TSource> query, Expression<Func<TSource, TRelated>> selector)
        {
            return new SDataQueryable<TSource>(
                query.Provider,
                Expression.Call(
                    new Func<IQueryable<TSource>, Expression<Func<TSource, TRelated>>, IQueryable<TSource>>(Fetch).Method,
                    query.Expression,
                    selector));
        }
    }
}