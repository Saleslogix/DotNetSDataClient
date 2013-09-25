// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

#if !NET_3_5
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client.Linq
{
    internal class SDataQueryExecutor
#if NET_3_5
        : IQueryExecutor
#else
        : IAsyncQueryExecutor
#endif
    {
        private readonly ISDataClient _client;
        private readonly string _path;
        private readonly INamingScheme _namingScheme;

        public SDataQueryExecutor(ISDataClient client, string path = null, INamingScheme namingScheme = null)
        {
            _client = client;
            _path = path;
            _namingScheme = namingScheme ?? NamingScheme.Default;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            int? takeCount;
#if PCL || NETFX_CORE || SILVERLIGHT
            PreExecuteScalar(queryModel, out takeCount);
            var alternative = queryModel.ResultOperators.Last().ToString().TrimEnd('(', ')') + "Async";
            throw new PlatformNotSupportedException(string.Format("Synchronous execution not supported on this platform, please use {0} instead", alternative));
#else
            var allOperator = queryModel.ResultOperators.Last() as AllResultOperator;
            if (allOperator != null)
            {
                var countOperator = new LongCountResultOperator();
                queryModel.ResultOperators.Remove(allOperator);
                queryModel.ResultOperators.Add(countOperator);
                var count1 = ExecuteScalar<long>(queryModel);
                queryModel.BodyClauses.Add(new WhereClause(allOperator.Predicate));
                var count2 = ExecuteScalar<long>(queryModel);
                return (T) Convert.ChangeType(count1 == count2, typeof (T), CultureInfo.InvariantCulture);
            }

            var parms = PreExecuteScalar(queryModel, out takeCount);
            var startIndex = parms.StartIndex;
            parms.StartIndex = null;
            var collection = takeCount != 0
                                 ? _client.Execute<SDataCollection<object>>(parms).Content
                                 : new SDataCollection<object>(0) {TotalResults = 0};
            return PostExecuteScalar<T>(queryModel, collection, startIndex, takeCount);
#endif
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            int? takeCount;
#if PCL || NETFX_CORE || SILVERLIGHT
            PreExecuteScalar(queryModel, out takeCount);
            var alternative = queryModel.ResultOperators.Last().ToString().TrimEnd('(', ')') + "Async";
            throw new PlatformNotSupportedException(string.Format("Synchronous execution not supported on this platform, please use {0} instead", alternative));
#else
            var lastOperator = queryModel.ResultOperators.Last() as LastResultOperator;
            if (lastOperator != null)
            {
                queryModel.ResultOperators.Remove(lastOperator);

                var orderByClauses = queryModel.BodyClauses.OfType<OrderByClause>().ToList();
                if (orderByClauses.Count > 0)
                {
                    foreach (var clause in orderByClauses)
                    {
                        foreach (var ordering in clause.Orderings)
                        {
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
                            ordering.OrderingDirection ^= OrderingDirection.Desc;
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
                        }
                    }
                }
                else
                {
                    var countOperator = new CountResultOperator();
                    queryModel.ResultOperators.Add(countOperator);
                    var count = ExecuteScalar<int>(queryModel);
                    queryModel.ResultOperators.Remove(countOperator);
                    if (count == 0)
                    {
                        queryModel.ResultOperators.Add(new TakeResultOperator(Expression.Constant(0)));
                    }
                    else
                    {
                        queryModel.ResultOperators.Add(new SkipResultOperator(Expression.Constant(count - 1)));
                    }
                }

                queryModel.ResultOperators.Add(new FirstResultOperator(lastOperator.ReturnDefaultWhenEmpty));
            }

            var parms = PreExecuteSingle(queryModel, out takeCount);
            var collection = takeCount != 0
                                 ? PrepareExecuteDelegate<T>(queryModel)(parms)
                                 : new SDataCollection<T>(0) {TotalResults = 0};
            return PostExecuteSingle(queryModel, collection);
#endif
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
#if PCL || NETFX_CORE || SILVERLIGHT
            throw new PlatformNotSupportedException("Synchronous execution not supported on this platform");
#else
            int? takeCount;
            var parms = PreExecuteCollection(queryModel, out takeCount);
            var execute = PrepareExecuteDelegate<T>(queryModel);

            while (true)
            {
                var collection = execute(parms);
                if (collection.Count == 0)
                {
                    yield break;
                }

                foreach (var item in collection)
                {
                    yield return item;

                    if (takeCount != null && --takeCount == 0)
                    {
                        yield break;
                    }
                }

                parms.StartIndex = (parms.StartIndex ?? 1) + collection.Count;
                if (collection.TotalResults != null && parms.StartIndex > collection.TotalResults)
                {
                    yield break;
                }

                if (takeCount != null && takeCount < parms.Count)
                {
                    parms.Count = takeCount;
                }
            }
#endif
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        private Func<ISDataParameters, SDataCollection<T>> PrepareExecuteDelegate<T>(QueryModel queryModel)
        {
            if (queryModel.MainFromClause.ItemType != typeof (T))
            {
                //this.Execute<[queryModel.MainFromClause.ItemType], T>(parms, [queryModel.SelectClause.Selector]);
                var executeMethod = new Func<ISDataParameters, Func<object, T>, SDataCollection<T>>(Execute)
                    .Method.GetGenericMethodDefinition()
                    .MakeGenericMethod(queryModel.MainFromClause.ItemType, typeof (T));
                var parmsParamExpr = Expression.Parameter(typeof (ISDataParameters), "parms");
                var selectorParamExpr = Expression.Parameter(queryModel.MainFromClause.ItemType, "selector");
                var mapping = new QuerySourceMapping();
                mapping.AddMapping(queryModel.MainFromClause, selectorParamExpr);
                var selector = ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(queryModel.SelectClause.Selector, mapping, true);
                var selectorFunc = Expression.Lambda(typeof (Func<,>).MakeGenericType(queryModel.MainFromClause.ItemType, typeof (T)), selector, selectorParamExpr).Compile();
                var lambdaExpr = Expression.Lambda<Func<ISDataParameters, SDataCollection<T>>>(
                    Expression.Call(
                        Expression.Constant(this),
                        executeMethod,
                        parmsParamExpr,
                        Expression.Constant(selectorFunc)),
                    parmsParamExpr);
                return lambdaExpr.Compile();
            }

            return Execute<T>;
        }

        private SDataCollection<TResult> Execute<TSource, TResult>(ISDataParameters parms, Func<TSource, TResult> selector)
        {
            var collection = Execute<TSource>(parms);
            return PostExecute(collection, selector);
        }

        private SDataCollection<T> Execute<T>(ISDataParameters parms)
        {
            return _client.Execute<SDataCollection<T>>(parms).Content;
        }
#endif

#if !NET_3_5
        public Task<T> ExecuteScalarAsync<T>(QueryModel queryModel)
        {
            var allOperator = queryModel.ResultOperators.Last() as AllResultOperator;
            if (allOperator != null)
            {
                var countOperator = new LongCountResultOperator();
                queryModel.ResultOperators.Remove(allOperator);
                queryModel.ResultOperators.Add(countOperator);
                return ExecuteScalarAsync<long>(queryModel)
                    .ContinueWith(
                        task1 =>
                            {
                                var count1 = task1.Result;
                                queryModel.BodyClauses.Add(new WhereClause(allOperator.Predicate));
                                return ExecuteScalarAsync<long>(queryModel)
                                    .ContinueWith(
                                        task2 =>
                                            {
                                                var count2 = task2.Result;
                                                return (T) Convert.ChangeType(count1 == count2, typeof (T), CultureInfo.InvariantCulture);
                                            });
                            }).Unwrap();
            }

            int? takeCount;
            var parms = PreExecuteScalar(queryModel, out takeCount);
            var startIndex = parms.StartIndex;
            if (takeCount == 0)
            {
                var collection = new SDataCollection<object>(0) {TotalResults = 0};
                return CreateResultTask(PostExecuteScalar<T>(queryModel, collection, startIndex, takeCount));
            }

            parms.StartIndex = null;
            return _client.ExecuteAsync<SDataCollection<object>>(parms)
                          .ContinueWith(task => PostExecuteScalar<T>(queryModel, task.Result.Content, startIndex, takeCount));
        }

        public Task<T> ExecuteSingleAsync<T>(QueryModel queryModel)
        {
            var lastOperator = queryModel.ResultOperators.Last() as LastResultOperator;
            if (lastOperator != null)
            {
                queryModel.ResultOperators.Remove(lastOperator);

                var orderByClauses = queryModel.BodyClauses.OfType<OrderByClause>().ToList();
                if (orderByClauses.Count > 0)
                {
                    foreach (var clause in orderByClauses)
                    {
                        foreach (var ordering in clause.Orderings)
                        {
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
                            ordering.OrderingDirection ^= OrderingDirection.Desc;
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
                        }
                    }
                }
                else
                {
                    var countOperator = new CountResultOperator();
                    queryModel.ResultOperators.Add(countOperator);
                    return ExecuteScalarAsync<int>(queryModel)
                        .ContinueWith(
                            task =>
                                {
                                    var count = task.Result;
                                    queryModel.ResultOperators.Remove(countOperator);
                                    if (count == 0)
                                    {
                                        queryModel.ResultOperators.Add(new TakeResultOperator(Expression.Constant(0)));
                                    }
                                    else
                                    {
                                        queryModel.ResultOperators.Add(new SkipResultOperator(Expression.Constant(count - 1)));
                                    }
                                    queryModel.ResultOperators.Add(new FirstResultOperator(lastOperator.ReturnDefaultWhenEmpty));
                                    return ExecuteSingleAsync<T>(queryModel);
                                }).Unwrap();
                }

                queryModel.ResultOperators.Add(new FirstResultOperator(lastOperator.ReturnDefaultWhenEmpty));
            }

            int? takeCount;
            var parms = PreExecuteSingle(queryModel, out takeCount);
            if (takeCount == 0)
            {
                var collection = new SDataCollection<T>(0) {TotalResults = 0};
                return CreateResultTask(PostExecuteSingle(queryModel, collection));
            }

            var execute = PrepareAsyncExecuteDelegate<T>(queryModel);
            return execute(parms).ContinueWith(task => PostExecuteSingle(queryModel, task.Result));
        }

        public Task<ICollection<T>> ExecuteCollectionAsync<T>(QueryModel queryModel)
        {
            int? takeCount;
            var parms = PreExecuteCollection(queryModel, out takeCount);
            var execute = PrepareAsyncExecuteDelegate<T>(queryModel);
            ICollection<T> items = new List<T>();
            Func<Task<SDataCollection<T>>> loop = null;
            loop =
                () => execute(parms)
                          .ContinueWith(
                              task =>
                                  {
                                      var collection = task.Result;
                                      if (collection.Count == 0)
                                      {
                                          return task;
                                      }

                                      foreach (var item in collection)
                                      {
                                          items.Add(item);
                                          if (takeCount != null && --takeCount == 0)
                                          {
                                              return task;
                                          }
                                      }

                                      parms.StartIndex = (parms.StartIndex ?? 1) + collection.Count;
                                      if (collection.TotalResults != null && parms.StartIndex > collection.TotalResults)
                                      {
                                          return task;
                                      }

                                      if (takeCount != null && takeCount < parms.Count)
                                      {
                                          parms.Count = takeCount;
                                      }

                                      return loop();
                                  })
                          .Unwrap();
            return loop().ContinueWith(task => items);
        }

        private Func<ISDataParameters, Task<SDataCollection<T>>> PrepareAsyncExecuteDelegate<T>(QueryModel queryModel)
        {
            if (queryModel.MainFromClause.ItemType != typeof (T))
            {
                //this.ExecuteAsync<[queryModel.MainFromClause.ItemType], T>(parms, [queryModel.SelectClause.Selector]);
                var executeMethod = new Func<ISDataParameters, Func<object, T>, Task<SDataCollection<T>>>(ExecuteAsync)
                    .GetMethodInfo()
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(queryModel.MainFromClause.ItemType, typeof (T));
                var parmsParamExpr = Expression.Parameter(typeof (ISDataParameters), "parms");
                var selectorParamExpr = Expression.Parameter(queryModel.MainFromClause.ItemType, "selector");
                var mapping = new QuerySourceMapping();
                mapping.AddMapping(queryModel.MainFromClause, selectorParamExpr);
                var selector = ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(queryModel.SelectClause.Selector, mapping, true);
                var selectorFunc = Expression.Lambda(typeof (Func<,>).MakeGenericType(queryModel.MainFromClause.ItemType, typeof (T)), selector, selectorParamExpr).Compile();
                var lambdaExpr = Expression.Lambda<Func<ISDataParameters, Task<SDataCollection<T>>>>(
                    Expression.Call(
                        Expression.Constant(this),
                        executeMethod,
                        parmsParamExpr,
                        Expression.Constant(selectorFunc)),
                    parmsParamExpr);
                return lambdaExpr.Compile();
            }

            return ExecuteAsync<T>;
        }

        private Task<SDataCollection<TResult>> ExecuteAsync<TSource, TResult>(ISDataParameters parms, Func<TSource, TResult> selector)
        {
            return ExecuteAsync<TSource>(parms)
                .ContinueWith(task => PostExecute(task.Result, selector));
        }

        private Task<SDataCollection<T>> ExecuteAsync<T>(ISDataParameters parms)
        {
            return _client.ExecuteAsync<SDataCollection<T>>(parms)
                          .ContinueWith(task => task.Result.Content);
        }

        private static Task<T> CreateResultTask<T>(T result)
        {
            var taskSource = new TaskCompletionSource<T>();
            taskSource.SetResult(result);
            return taskSource.Task;
        }
#endif

        private ISDataParameters PreExecuteScalar(QueryModel queryModel, out int? takeCount)
        {
            var resultOperator = queryModel.ResultOperators.Last();
            if (!(resultOperator is AnyResultOperator ||
                  resultOperator is CountResultOperator ||
                  resultOperator is LongCountResultOperator))
            {
                throw new NotSupportedException();
            }

            var parms = CreateParameters(queryModel);
            takeCount = parms.Count;
            parms.Count = 0;
            return parms;
        }

        private static T PostExecuteScalar<T>(QueryModel queryModel, SDataCollection<object> collection, int? startIndex, int? takeCount)
        {
            var totalResults = collection.TotalResults;
            if (totalResults == null)
            {
                throw new SDataClientException("Unable to determine total results");
            }

            if (startIndex != null)
            {
                totalResults = Math.Max(totalResults.Value - startIndex.Value + 1, 0);
            }
            if (takeCount != null)
            {
                totalResults = Math.Min(totalResults.Value, takeCount.Value);
            }

            var resultOperator = queryModel.ResultOperators.Last();
            var obj = resultOperator is AnyResultOperator
                          ? totalResults > 0
                          : (object) totalResults;
            return (T) Convert.ChangeType(obj, typeof (T), CultureInfo.InvariantCulture);
        }

        private ISDataParameters PreExecuteSingle(QueryModel queryModel, out int? takeCount)
        {
            var resultOperator = queryModel.ResultOperators.Last();
            var elementAtOperator = resultOperator as ElementAtResultOperator;
            if (elementAtOperator != null)
            {
                queryModel.ResultOperators.Remove(elementAtOperator);
                queryModel.ResultOperators.Add(new SkipResultOperator(Expression.Constant(elementAtOperator.Index)));
                queryModel.ResultOperators.Add(new FirstResultOperator(elementAtOperator.ReturnDefaultWhenEmpty));
            }
            else if (!(resultOperator is FirstResultOperator ||
                       resultOperator is SingleResultOperator))
            {
                throw new NotSupportedException();
            }

            var parms = CreateParameters(queryModel);
            takeCount = parms.Count;
            parms.Count = 1;
            return parms;
        }

        private static T PostExecuteSingle<T>(QueryModel queryModel, SDataCollection<T> collection)
        {
            var resultOperator = queryModel.ResultOperators.Last();
            if (resultOperator is SingleResultOperator)
            {
                if (collection.TotalResults == null)
                {
                    throw new SDataClientException("Unable to determine total results");
                }

                if (collection.TotalResults > 1)
                {
                    throw new SDataClientException("Multiple results");
                }
            }

            var sequence = new StreamedSequence(collection, new StreamedSequenceInfo(typeof (IEnumerable<T>), queryModel.SelectClause.Selector));
            return ((ValueFromSequenceResultOperatorBase) resultOperator).ExecuteInMemory<T>(sequence).GetTypedValue<T>();
        }

        private ISDataParameters PreExecuteCollection(QueryModel queryModel, out int? takeCount)
        {
            var parms = CreateParameters(queryModel);

            takeCount = parms.Count;
            if (takeCount > 100)
            {
                parms.Count = 100;
            }

            return parms;
        }

        private static SDataCollection<TResult> PostExecute<TSource, TResult>(SDataCollection<TSource> collection, Func<TSource, TResult> selector)
        {
            var projected = new SDataCollection<TResult>(collection.Select(selector));
            ((ISDataProtocolAware) projected).Info = ((ISDataProtocolAware) collection).Info;
            return projected;
        }

        private ISDataParameters CreateParameters(QueryModel queryModel)
        {
            var visitor = new SDataQueryModelVisitor(_namingScheme);
            visitor.VisitQueryModel(queryModel);
            return new SDataParameters
                       {
                           Path = _path ?? SDataResourceAttribute.GetPath(visitor.MainType) ?? _namingScheme.GetName(visitor.MainType.GetTypeInfo()),
                           Select = visitor.Select,
                           Where = visitor.Where,
                           OrderBy = visitor.OrderBy,
                           Count = visitor.Count,
                           StartIndex = visitor.StartIndex,
                           Include = visitor.Include,
                           Precedence = visitor.Precedence
                       };
        }
    }
}