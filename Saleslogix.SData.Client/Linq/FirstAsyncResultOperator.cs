﻿// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class FirstAsyncResultOperator : FirstResultOperator
    {
        private readonly CancellationToken _cancel;

        public FirstAsyncResultOperator(bool returnDefaultWhenEmpty, CancellationToken cancel)
            : base(returnDefaultWhenEmpty)
        {
            _cancel = cancel;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new FirstAsyncResultOperator(ReturnDefaultWhenEmpty, _cancel);
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
            return new StreamedAsyncSingleInfo(inputSequenceInfo.ResultItemType, _cancel);
        }

        public override string ToString()
        {
            return ReturnDefaultWhenEmpty ? "FirstOrDefaultAsync()" : "FirstAsync()";
        }
    }
}