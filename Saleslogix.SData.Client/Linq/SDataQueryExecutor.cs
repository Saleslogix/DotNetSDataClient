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
using Remotion.Linq.Parsing.Structure;

#if !NET_3_5
using System.Threading;
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
        private readonly INodeTypeProvider _nodeTypeProvider;
        private readonly ISDataClient _client;
        private readonly string _path;
        private readonly INamingScheme _namingScheme;

        public SDataQueryExecutor(ISDataClient client, string path = null, INodeTypeProvider nodeTypeProvider = null, INamingScheme namingScheme = null)
        {
            _client = client;
            _path = path;
            _nodeTypeProvider = nodeTypeProvider ?? ExpressionTreeParser.CreateDefaultNodeTypeProvider();
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
            return PostExecuteSingle(queryModel, collection, parms.StartIndex, takeCount);
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
        private Func<SDataParameters, SDataCollection<T>> PrepareExecuteDelegate<T>(QueryModel queryModel)
        {
            if (queryModel.MainFromClause.ItemType != typeof (T))
            {
                //this.Execute<[queryModel.MainFromClause.ItemType], T>(parms, [queryModel.SelectClause.Selector]);
                var executeMethod = new Func<SDataParameters, Func<object, T>, SDataCollection<T>>(Execute)
                    .GetMethodInfo()
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(queryModel.MainFromClause.ItemType, typeof (T));
                var parmsParamExpr = Expression.Parameter(typeof (SDataParameters), "parms");
                var selectorParamExpr = Expression.Parameter(queryModel.MainFromClause.ItemType, "selector");
                var mapping = new QuerySourceMapping();
                mapping.AddMapping(queryModel.MainFromClause, selectorParamExpr);
                var selector = ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(queryModel.SelectClause.Selector, mapping, true);
                var selectorFunc = Expression.Lambda(typeof (Func<,>).MakeGenericType(queryModel.MainFromClause.ItemType, typeof (T)), selector, selectorParamExpr).Compile();
                var lambdaExpr = Expression.Lambda<Func<SDataParameters, SDataCollection<T>>>(
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

        private SDataCollection<TResult> Execute<TSource, TResult>(SDataParameters parms, Func<TSource, TResult> selector)
        {
            var collection = Execute<TSource>(parms);
            return PostExecute(collection, selector);
        }

        private SDataCollection<T> Execute<T>(SDataParameters parms)
        {
            return _client.Execute<SDataCollection<T>>(parms).Content;
        }
#endif

#if !NET_3_5
        public Task<T> ExecuteScalarAsync<T>(QueryModel queryModel, CancellationToken cancel)
        {
            var allOperator = queryModel.ResultOperators.Last() as AllResultOperator;
            if (allOperator != null)
            {
                var countOperator = new LongCountResultOperator();
                queryModel.ResultOperators.Remove(allOperator);
                queryModel.ResultOperators.Add(countOperator);
                return ExecuteScalarAsync<long>(queryModel, cancel)
                    .ContinueWith(
                        task1 =>
                            {
                                var count1 = task1.Result;
                                queryModel.BodyClauses.Add(new WhereClause(allOperator.Predicate));
                                return ExecuteScalarAsync<long>(queryModel, cancel)
                                    .ContinueWith(
                                        task2 =>
                                            {
                                                var count2 = task2.Result;
                                                return (T) Convert.ChangeType(count1 == count2, typeof (T), CultureInfo.InvariantCulture);
                                            }, cancel);
                            }, cancel).Unwrap();
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
            return _client.ExecuteAsync<SDataCollection<object>>(parms, cancel)
                          .ContinueWith(task => PostExecuteScalar<T>(queryModel, task.Result.Content, startIndex, takeCount), cancel);
        }

        public Task<T> ExecuteSingleAsync<T>(QueryModel queryModel, CancellationToken cancel)
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
                    return ExecuteScalarAsync<int>(queryModel, cancel)
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
                                    return ExecuteSingleAsync<T>(queryModel, cancel);
                                }, cancel).Unwrap();
                }

                queryModel.ResultOperators.Add(new FirstResultOperator(lastOperator.ReturnDefaultWhenEmpty));
            }

            int? takeCount;
            var parms = PreExecuteSingle(queryModel, out takeCount);
            if (takeCount == 0)
            {
                var collection = new SDataCollection<T>(0) {TotalResults = 0};
                return CreateResultTask(PostExecuteSingle(queryModel, collection, parms.StartIndex, takeCount));
            }

            var execute = PrepareAsyncExecuteDelegate<T>(queryModel);
            return execute(parms, cancel).ContinueWith(task => PostExecuteSingle(queryModel, task.Result, parms.StartIndex, takeCount), cancel);
        }

        public Task<ICollection<T>> ExecuteCollectionAsync<T>(QueryModel queryModel, CancellationToken cancel)
        {
            int? takeCount;
            var parms = PreExecuteCollection(queryModel, out takeCount);
            var execute = PrepareAsyncExecuteDelegate<T>(queryModel);
            ICollection<T> items = new List<T>();
            Func<Task<SDataCollection<T>>> loop = null;
            loop =
                () => execute(parms, cancel)
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
                                  }, cancel)
                          .Unwrap();
            return loop().ContinueWith(task => items, cancel);
        }

        private Func<SDataParameters, CancellationToken, Task<SDataCollection<T>>> PrepareAsyncExecuteDelegate<T>(QueryModel queryModel)
        {
            if (queryModel.MainFromClause.ItemType != typeof (T))
            {
                //this.ExecuteAsync<[queryModel.MainFromClause.ItemType], T>(parms, [queryModel.SelectClause.Selector], cancel);
                var executeMethod = new Func<SDataParameters, Func<object, T>, CancellationToken, Task<SDataCollection<T>>>(ExecuteAsync)
                    .GetMethodInfo()
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(queryModel.MainFromClause.ItemType, typeof (T));
                var parmsParamExpr = Expression.Parameter(typeof (SDataParameters), "parms");
                var selectorParamExpr = Expression.Parameter(queryModel.MainFromClause.ItemType, "selector");
                var cancelParamExpr = Expression.Parameter(typeof (CancellationToken), "cancel");
                var mapping = new QuerySourceMapping();
                mapping.AddMapping(queryModel.MainFromClause, selectorParamExpr);
                var selector = ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(queryModel.SelectClause.Selector, mapping, true);
                var selectorFunc = Expression.Lambda(typeof (Func<,>).MakeGenericType(queryModel.MainFromClause.ItemType, typeof (T)), selector, selectorParamExpr).Compile();
                var lambdaExpr = Expression.Lambda<Func<SDataParameters, CancellationToken, Task<SDataCollection<T>>>>(
                    Expression.Call(
                        Expression.Constant(this),
                        executeMethod,
                        parmsParamExpr,
                        Expression.Constant(selectorFunc),
                        cancelParamExpr),
                    parmsParamExpr,
                    cancelParamExpr);
                return lambdaExpr.Compile();
            }

            return ExecuteAsync<T>;
        }

        private Task<SDataCollection<TResult>> ExecuteAsync<TSource, TResult>(SDataParameters parms, Func<TSource, TResult> selector, CancellationToken cancel)
        {
            return ExecuteAsync<TSource>(parms, cancel)
                .ContinueWith(task => PostExecute(task.Result, selector), cancel);
        }

        private Task<SDataCollection<T>> ExecuteAsync<T>(SDataParameters parms, CancellationToken cancel)
        {
            return _client.ExecuteAsync<SDataCollection<T>>(parms, cancel)
                          .ContinueWith(task => task.Result.Content, cancel);
        }

        private static Task<T> CreateResultTask<T>(T result)
        {
            var taskSource = new TaskCompletionSource<T>();
            taskSource.SetResult(result);
            return taskSource.Task;
        }
#endif

        private SDataParameters PreExecuteScalar(QueryModel queryModel, out int? takeCount)
        {
            var resultOperator = queryModel.ResultOperators.Last();
            if (!(resultOperator is AnyResultOperator ||
                  resultOperator is CountResultOperator ||
                  resultOperator is LongCountResultOperator))
            {
                throw new NotSupportedException(string.Format("Result operator '{0}' not supported", resultOperator.GetType()));
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

        private SDataParameters PreExecuteSingle(QueryModel queryModel, out int? takeCount)
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
                throw new NotSupportedException(string.Format("Result operator '{0}' not supported", resultOperator.GetType()));
            }

            var parms = CreateParameters(queryModel);
            takeCount = parms.Count;
            parms.Count = 1;
            return parms;
        }

        private static T PostExecuteSingle<T>(QueryModel queryModel, SDataCollection<T> collection, int? startIndex, int? takeCount)
        {
            var resultOperator = queryModel.ResultOperators.Last();
            if (resultOperator is SingleResultOperator && takeCount != 1)
            {
                if (collection.TotalResults == null)
                {
                    throw new SDataClientException("Unable to determine total results");
                }

                if (collection.TotalResults - (startIndex ?? 1) > 0)
                {
                    throw new SDataClientException("Multiple results found");
                }
            }

            var sequence = new StreamedSequence(collection, new StreamedSequenceInfo(typeof (IEnumerable<T>), queryModel.SelectClause.Selector));
            return ((ValueFromSequenceResultOperatorBase) resultOperator).ExecuteInMemory<T>(sequence).GetTypedValue<T>();
        }

        private SDataParameters PreExecuteCollection(QueryModel queryModel, out int? takeCount)
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

        private SDataParameters CreateParameters(QueryModel queryModel)
        {
            var visitor = new SDataQueryModelVisitor(_nodeTypeProvider, _namingScheme);
            visitor.VisitQueryModel(queryModel);
            var path = _path ?? SDataResourceAttribute.GetPath(visitor.MainType);
            var parms = new SDataParameters
                       {
                           Path = path,
                           Select = visitor.Select,
                           Where = visitor.Where,
                           OrderBy = visitor.OrderBy,
                           Count = visitor.Count,
                           StartIndex = visitor.StartIndex,
                           Include = visitor.Include,
                           Precedence = visitor.Precedence
                       };
            foreach (var item in visitor.ExtensionArgs)
            {
                parms.ExtensionArgs[item.Key] = item.Value;
            }
            return parms;
        }
    }
}