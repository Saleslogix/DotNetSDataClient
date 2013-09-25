using System.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class LastAsyncResultOperator : LastResultOperator
    {
        public LastAsyncResultOperator(bool returnDefaultWhenEmpty)
            : base(returnDefaultWhenEmpty)
        {
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new LastAsyncResultOperator(ReturnDefaultWhenEmpty);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = ReturnDefaultWhenEmpty ? sequence.LastOrDefault() : sequence.Last();
            return new StreamedValue(result, (StreamedValueInfo) base.GetOutputDataInfo(input.DataInfo));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            var inputSequenceInfo = ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncSingleInfo(inputSequenceInfo.ResultItemType);
        }

        public override string ToString()
        {
            return ReturnDefaultWhenEmpty ? "LastOrDefaultAsync()" : "LastAsync()";
        }
    }
}