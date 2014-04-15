// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class SingleAsyncResultOperator : SingleResultOperator
    {
        private readonly CancellationToken _cancel;

        public SingleAsyncResultOperator(bool returnDefaultWhenEmpty, CancellationToken cancel)
            : base(returnDefaultWhenEmpty)
        {
            _cancel = cancel;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new SingleAsyncResultOperator(ReturnDefaultWhenEmpty, _cancel);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = ReturnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
            return new StreamedValue(result, (StreamedValueInfo) base.GetOutputDataInfo(input.DataInfo));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            var inputSequenceInfo = ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncSingleInfo(inputSequenceInfo.ResultItemType, _cancel);
        }

        public override string ToString()
        {
            return ReturnDefaultWhenEmpty ? "SingleOrDefaultAsync()" : "SingleAsync()";
        }
    }
}