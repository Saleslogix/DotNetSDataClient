// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Saleslogix.SData.Client.Utilities;

#if !NET_3_5
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client.Linq
{
    public static class SDataQueryableExtensions
    {
        public static IQueryable<T> WithPrecedence<T>(this IQueryable<T> source, int value)
        {
            Guard.ArgumentNotNull(source, "source");
            return new SDataQueryable<T>(
                source.Provider,
                Expression.Call(
                    new Func<IQueryable<T>, int, IQueryable<T>>(WithPrecedence).GetMethodInfo(),
                    source.Expression,
                    Expression.Constant(value)));
        }

        public static IQueryable<T> WithExtensionArg<T>(this IQueryable<T> source, string name, string value)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNullOrEmptyString(name, "name");
            return new SDataQueryable<T>(
                source.Provider,
                Expression.Call(
                    new Func<IQueryable<T>, string, string, IQueryable<T>>(WithExtensionArg).GetMethodInfo(),
                    source.Expression,
                    Expression.Constant(name),
                    Expression.Constant(value)));
        }

        public static IQueryable<TSource> Fetch<TSource, TRelated>(this IQueryable<TSource> source, Expression<Func<TSource, TRelated>> selector)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(selector, "selector");
            return new SDataQueryable<TSource>(
                source.Provider,
                Expression.Call(
                    new Func<IQueryable<TSource>, Expression<Func<TSource, TRelated>>, IQueryable<TSource>>(Fetch).GetMethodInfo(),
                    source.Expression,
                    selector));
        }

#if !NET_3_5
        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.ToCollectionAsync(cancel).ContinueWith(task => task.Result.ToList(), cancel);
        }

        public static Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.ToCollectionAsync(cancel).ContinueWith(task => task.Result.ToArray(), cancel);
        }

        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null,
            CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(keySelector, "keySelector");
            return source.ToCollectionAsync(cancel).ContinueWith(task => task.Result.ToDictionary(keySelector, comparer), cancel);
        }

        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null,
            CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(keySelector, "keySelector");
            return source.ToCollectionAsync(cancel).ContinueWith(task => task.Result.ToDictionary(keySelector, elementSelector, comparer), cancel);
        }

        public static Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null,
            CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(keySelector, "keySelector");
            return source.ToCollectionAsync(cancel).ContinueWith(task => task.Result.ToLookup(keySelector, comparer), cancel);
        }

        public static Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null,
            CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(keySelector, "keySelector");
            return source.ToCollectionAsync(cancel).ContinueWith(task => task.Result.ToLookup(keySelector, elementSelector, comparer), cancel);
        }

        internal static Task<ICollection<TSource>> ToCollectionAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            return source.Provider.Execute<Task<ICollection<TSource>>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<ICollection<TSource>>>(ToCollectionAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<int>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<int>>(CountAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<int>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<int>>(CountAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<long>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<long>>(LongCountAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<long>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<long>>(LongCountAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<bool>>(AnyAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>(AnyAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<bool>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>(AllAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<TSource>>(FirstAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(FirstAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<TSource>>(FirstOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(FirstOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<TSource>>(LastAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(LastAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<TSource>>(LastOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(LastOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<TSource>>(SingleAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(SingleAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, CancellationToken, Task<TSource>>(SingleOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(cancel)}));
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(SingleOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Quote(predicate), Expression.Constant(cancel)}));
        }

        public static Task<TSource> ElementAtAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, int, CancellationToken, Task<TSource>>(ElementAtAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(index), Expression.Constant(cancel)}));
        }

        public static Task<TSource> ElementAtOrDefaultAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Provider.Execute<Task<TSource>>(
                Expression.Call(null,
                                new Func<IQueryable<TSource>, int, CancellationToken, Task<TSource>>(ElementAtOrDefaultAsync).GetMethodInfo(),
                                new[] {source.Expression, Expression.Constant(index), Expression.Constant(cancel)}));
        }
#endif
    }
}