﻿// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class AnyAsyncResultOperator : AnyResultOperator
    {
        private readonly CancellationToken _cancel;

        public AnyAsyncResultOperator(CancellationToken cancel)
        {
            _cancel = cancel;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new AnyAsyncResultOperator(_cancel);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = sequence.Any();
            return new StreamedValue(result, (StreamedValueInfo) base.GetOutputDataInfo(input.DataInfo));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncScalarInfo(typeof (bool), _cancel);
        }

        public override string ToString()
        {
            return "AnyAsync()";
        }
    }
}