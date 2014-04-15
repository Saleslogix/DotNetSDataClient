// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class AllAsyncResultOperator : AllResultOperator
    {
        private readonly Expression _predicate;
        private readonly CancellationToken _cancel;

        public AllAsyncResultOperator(Expression predicate, CancellationToken cancel)
            : base(predicate)
        {
            _predicate = predicate;
            _cancel = cancel;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new AllAsyncResultOperator(_predicate, _cancel);
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var predicateLambda = ReverseResolvingExpressionTreeVisitor.ReverseResolve(input.DataInfo.ItemExpression, Predicate);
            var predicate = (Func<T, bool>) predicateLambda.Compile();
            var result = sequence.All(predicate);
            return new StreamedValue(result, (StreamedValueInfo) base.GetOutputDataInfo(input.DataInfo));
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            ArgumentUtility.CheckNotNullAndType<StreamedSequenceInfo>("inputInfo", inputInfo);
            return new StreamedAsyncScalarInfo(typeof (bool), _cancel);
        }

        public override string ToString()
        {
            return string.Format("AllAsync({0})", FormattingExpressionTreeVisitor.Format(_predicate));
        }
    }
}