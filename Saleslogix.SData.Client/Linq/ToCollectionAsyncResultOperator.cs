// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class ToCollectionAsyncResultOperator : ValueFromSequenceResultOperatorBase
    {
        private readonly CancellationToken _cancel;

        public ToCollectionAsyncResultOperator(CancellationToken cancel)
        {
            _cancel = cancel;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new ToCollectionAsyncResultOperator(_cancel);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = sequence.ToList();
            return new StreamedValue(result, new StreamedScalarValueInfo(typeof (ICollection<>).MakeGenericType(input.DataInfo.ResultItemType)));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            var inputSequenceInfo = ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncCollectionInfo(inputSequenceInfo.ResultItemType, _cancel);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return "ToCollectionAsync()";
        }
    }
}