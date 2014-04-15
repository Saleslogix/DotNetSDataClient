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
    internal class AnyAsyncExpressionNode : AnyExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<bool>>(SDataQueryableExtensions.AnyAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public AnyAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate, ConstantExpression optionalCancel)
            : base(parseInfo, predicate)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new AnyAsyncResultOperator(_cancel);
        }
    }

    internal class AnyAsyncWithoutPredicateExpressionNode : AnyAsyncExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, CancellationToken, Task<bool>>(SDataQueryableExtensions.AnyAsync).GetMethodInfo().GetGenericMethodDefinition()
            };

        public AnyAsyncWithoutPredicateExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression optionalCancel)
            : base(parseInfo, null, optionalCancel)
        {
        }
    }
}