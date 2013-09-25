// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class SingleAsyncResultOperator : SingleResultOperator
    {
        public SingleAsyncResultOperator(bool returnDefaultWhenEmpty)
            : base(returnDefaultWhenEmpty)
        {
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new SingleAsyncResultOperator(ReturnDefaultWhenEmpty);
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
            return new StreamedAsyncSingleInfo(inputSequenceInfo.ResultItemType);
        }

        public override string ToString()
        {
            return ReturnDefaultWhenEmpty ? "SingleOrDefaultAsync()" : "SingleAsync()";
        }
    }
}