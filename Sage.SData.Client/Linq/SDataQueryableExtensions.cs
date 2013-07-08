using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#if !NET_3_5
using System.Collections.Generic;
using System.Threading.Tasks;
#endif

namespace Sage.SData.Client.Linq
{
    public static class SDataQueryableExtensions
    {
        public static IQueryable<T> WithPrecedence<T>(this IQueryable<T> query, int value)
        {
            return new SDataQueryable<T>(
                query.Provider,
                Expression.Call(
                    new Func<IQueryable<T>, int, IQueryable<T>>(WithPrecedence).GetMethodInfo(),
                    query.Expression,
                    Expression.Constant(value)));
        }

        public static IQueryable<TSource> Fetch<TSource, TRelated>(this IQueryable<TSource> query, Expression<Func<TSource, TRelated>> selector)
        {
            return new SDataQueryable<TSource>(
                query.Provider,
                Expression.Call(
                    new Func<IQueryable<TSource>, Expression<Func<TSource, TRelated>>, IQueryable<TSource>>(Fetch).GetMethodInfo(),
                    query.Expression,
                    selector));
        }

#if !NET_3_5
        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.ToCollectionAsync().ContinueWith(task => task.Result.ToList());
        }

        public static Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.ToCollectionAsync().ContinueWith(task => task.Result.ToArray());
        }

        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            return source.ToCollectionAsync().ContinueWith(task => task.Result.ToDictionary(keySelector, comparer));
        }

        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            return source.ToCollectionAsync().ContinueWith(task => task.Result.ToDictionary(keySelector, elementSelector, comparer));
        }

        public static Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            return source.ToCollectionAsync().ContinueWith(task => task.Result.ToLookup(keySelector, comparer));
        }

        public static Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            return source.ToCollectionAsync().ContinueWith(task => task.Result.ToLookup(keySelector, elementSelector, comparer));
        }

        internal static Task<ICollection<TSource>> ToCollectionAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<ICollection<TSource>>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<ICollection<TSource>>>(ToCollectionAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<int>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<int>>(CountAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<int>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<int>>(CountAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<long>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<long>>(LongCountAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<long>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<long>>(LongCountAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<bool>>(AnyAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<bool>>(AnyAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<bool>>(AllAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<bool>>(AllAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<TSource>>(FirstAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<TSource>>(FirstAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<TSource>>(FirstOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<TSource>>(FirstOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<TSource>>(LastAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<TSource>>(LastAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<TSource>>(LastOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<TSource>>(LastOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<TSource>>(SingleAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<TSource>>(SingleAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Task<TSource>>(SingleOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression}));
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, Task<TSource>>(SingleOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate)}));
        }

        public static Task<TSource> ElementAtAsync<TSource>(this IQueryable<TSource> source, int index)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, int, Task<TSource>>(ElementAtAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(index)}));
        }

        public static Task<TSource> ElementAtOrDefaultAsync<TSource>(this IQueryable<TSource> source, int index)
        {
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, int, Task<TSource>>(ElementAtOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(index)}));
        }
#endif
    }
}