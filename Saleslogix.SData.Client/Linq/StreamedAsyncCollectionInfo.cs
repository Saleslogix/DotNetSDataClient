// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class StreamedAsyncCollectionInfo : StreamedValueInfo
    {
        private static readonly MethodInfo _executeMethod = new Func<QueryModel, IAsyncQueryExecutor, CancellationToken, object>(ExecuteCollectionQueryModel<object>).GetMethodInfo().GetGenericMethodDefinition();

        private readonly Type _itemType;
        private readonly CancellationToken _cancel;

        public StreamedAsyncCollectionInfo(Type itemType, CancellationToken cancel)
            : base(typeof (Task<>).MakeGenericType(typeof (ICollection<>).MakeGenericType(itemType)))
        {
            _itemType = itemType;
            _cancel = cancel;
        }

        public override IStreamedData ExecuteQueryModel(QueryModel queryModel, IQueryExecutor executor)
        {
            ArgumentUtility.CheckNotNull("queryModel", queryModel);
            var asyncExecutor = ArgumentUtility.CheckNotNullAndType<IAsyncQueryExecutor>("executor", executor);
            var executeMethod = _executeMethod.MakeGenericMethod(_itemType);
            var result = executeMethod.Invoke(null, new object[] {queryModel, asyncExecutor, _cancel});
            return new StreamedValue(result, this);
        }

        protected override StreamedValueInfo CloneWithNewDataType(Type dataType)
        {
            ArgumentUtility.CheckNotNull("dataType", dataType);
            if (dataType != DataType)
            {
                throw new NotSupportedException("Cloning StreamedAsyncCollectionInfo with a different data type not supported");
            }
            return new StreamedAsyncCollectionInfo(_itemType, _cancel);
        }

        private static object ExecuteCollectionQueryModel<T>(QueryModel queryModel, IAsyncQueryExecutor executor, CancellationToken cancel)
        {
            ArgumentUtility.CheckNotNull("queryModel", queryModel);
            ArgumentUtility.CheckNotNull("executor", executor);
            return executor.ExecuteCollectionAsync<T>(queryModel, cancel);
        }
    }
}