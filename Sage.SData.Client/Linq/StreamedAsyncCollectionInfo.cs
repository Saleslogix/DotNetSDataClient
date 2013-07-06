using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Sage.SData.Client.Linq
{
    internal class StreamedAsyncCollectionInfo : StreamedValueInfo
    {
        private static readonly MethodInfo _executeMethod = new Func<QueryModel, IAsyncQueryExecutor, object>(ExecuteCollectionQueryModel<object>).Method.GetGenericMethodDefinition();

        private readonly Type _itemType;

        public StreamedAsyncCollectionInfo(Type itemType)
            : base(typeof (Task<>).MakeGenericType(typeof (ICollection<>).MakeGenericType(itemType)))
        {
            _itemType = itemType;
        }

        public override IStreamedData ExecuteQueryModel(QueryModel queryModel, IQueryExecutor executor)
        {
            ArgumentUtility.CheckNotNull("queryModel", queryModel);
            var asyncExecutor = ArgumentUtility.CheckNotNullAndType<IAsyncQueryExecutor>("executor", executor);
            var executeMethod = _executeMethod.MakeGenericMethod(_itemType);
            var result = executeMethod.Invoke(null, new object[] {queryModel, asyncExecutor});
            return new StreamedValue(result, this);
        }

        protected override StreamedValueInfo CloneWithNewDataType(Type dataType)
        {
            ArgumentUtility.CheckNotNull("dataType", dataType);
            Debug.Assert(dataType == DataType);
            return new StreamedAsyncCollectionInfo(_itemType);
        }

        private static object ExecuteCollectionQueryModel<T>(QueryModel queryModel, IAsyncQueryExecutor executor)
        {
            ArgumentUtility.CheckNotNull("queryModel", queryModel);
            ArgumentUtility.CheckNotNull("executor", executor);
            return executor.ExecuteCollectionAsync<T>(queryModel);
        }
    }
}