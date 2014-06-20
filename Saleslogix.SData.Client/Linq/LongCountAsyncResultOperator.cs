// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class LongCountAsyncResultOperator : LongCountResultOperator
    {
        private readonly CancellationToken _cancel;

        public LongCountAsyncResultOperator(CancellationToken cancel)
        {
            _cancel = cancel;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new LongCountAsyncResultOperator(_cancel);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = sequence.LongCount();
            return new StreamedValue(result, (StreamedValueInfo) base.GetOutputDataInfo(input.DataInfo));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncScalarInfo(typeof (long), _cancel);
        }

        public override string ToString()
        {
            return "LongCountAsync()";
        }
    }
}