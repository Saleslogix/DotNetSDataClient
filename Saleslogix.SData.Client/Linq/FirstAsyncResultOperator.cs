// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class FirstAsyncResultOperator : FirstResultOperator
    {
        public FirstAsyncResultOperator(bool returnDefaultWhenEmpty)
            : base(returnDefaultWhenEmpty)
        {
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new FirstAsyncResultOperator(ReturnDefaultWhenEmpty);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = ReturnDefaultWhenEmpty ? sequence.FirstOrDefault() : sequence.First();
            return new StreamedValue(result, (StreamedValueInfo) base.GetOutputDataInfo(input.DataInfo));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            var inputSequenceInfo = ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncSingleInfo(inputSequenceInfo.ResultItemType);
        }

        public override string ToString()
        {
            return ReturnDefaultWhenEmpty ? "FirstOrDefaultAsync()" : "FirstAsync()";
        }
    }
}