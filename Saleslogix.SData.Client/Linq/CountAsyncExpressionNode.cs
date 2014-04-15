// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Saleslogix.SData.Client.Linq
{
    internal class CountAsyncExpressionNode : CountExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<int>>(SDataQueryableExtensions.CountAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public CountAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate, ConstantExpression optionalCancel)
            : base(parseInfo, predicate)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new CountAsyncResultOperator(_cancel);
        }
    }

    internal class CountAsyncWithoutPredicateExpressionNode : CountAsyncExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, CancellationToken, Task<int>>(SDataQueryableExtensions.CountAsync).GetMethodInfo().GetGenericMethodDefinition()
            };

        public CountAsyncWithoutPredicateExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression optionalCancel)
            : base(parseInfo, null, optionalCancel)
        {
        }
    }
}