using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Sage.SData.Client.Linq
{
    internal class AllAsyncResultOperator : AllResultOperator
    {
        private readonly Expression _predicate;

        public AllAsyncResultOperator(Expression predicate)
            : base(predicate)
        {
            _predicate = predicate;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new AllAsyncResultOperator(_predicate);
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
            return new StreamedAsyncScalarInfo(typeof (bool));
        }

        public override string ToString()
        {
            return string.Format("AllAsync({0})", FormattingExpressionTreeVisitor.Format(_predicate));
        }
    }
}