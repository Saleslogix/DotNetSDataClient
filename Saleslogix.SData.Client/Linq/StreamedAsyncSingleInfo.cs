using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class StreamedAsyncSingleInfo : StreamedValueInfo
    {
        private static readonly MethodInfo _executeMethod = new Func<QueryModel, IAsyncQueryExecutor, object>(ExecuteSingleQueryModel<object>).GetMethodInfo().GetGenericMethodDefinition();

        private readonly Type _resultType;

        public StreamedAsyncSingleInfo(Type resultType)
            : base(typeof (Task<>).MakeGenericType(resultType))
        {
            _resultType = resultType;
        }

        public override IStreamedData ExecuteQueryModel(QueryModel queryModel, IQueryExecutor executor)
        {
            ArgumentUtility.CheckNotNull("queryModel", queryModel);
            var asyncExecutor = ArgumentUtility.CheckNotNullAndType<IAsyncQueryExecutor>("executor", executor);
            var executeMethod = _executeMethod.MakeGenericMethod(_resultType);
            var result = executeMethod.Invoke(null, new object[] {queryModel, asyncExecutor});
            return new StreamedValue(result, this);
        }

        protected override StreamedValueInfo CloneWithNewDataType(Type dataType)
        {
            ArgumentUtility.CheckNotNull("dataType", dataType);
            Debug.Assert(dataType == DataType);
            return new StreamedAsyncSingleInfo(_resultType);
        }

        private static object ExecuteSingleQueryModel<T>(QueryModel queryModel, IAsyncQueryExecutor executor)
        {
            ArgumentUtility.CheckNotNull("queryModel", queryModel);
            ArgumentUtility.CheckNotNull("executor", executor);
            return executor.ExecuteSingleAsync<T>(queryModel);
        }
    }
}