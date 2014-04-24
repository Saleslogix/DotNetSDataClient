// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.StreamedData;

namespace Saleslogix.SData.Client.Linq
{
    internal class FetchResultOperator : ResultOperatorBase
    {
        public Expression Selector { get; private set; }

        public FetchResultOperator(Expression selector)
        {
            Selector = selector;
        }

        public override IStreamedData ExecuteInMemory(IStreamedData input)
        {
            return input;
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            return inputInfo;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new FetchResultOperator(Selector);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return string.Format("Fetch({0})", FormattingExpressionTreeVisitor.Format(Selector));
        }
    }
}