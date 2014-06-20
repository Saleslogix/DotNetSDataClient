// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

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
    internal class LongCountAsyncExpressionNode : LongCountExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<long>>(SDataQueryableExtensions.LongCountAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public LongCountAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate, ConstantExpression optionalCancel)
            : base(parseInfo, predicate)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new LongCountAsyncResultOperator(_cancel);
        }
    }

    internal class LongCountAsyncWithoutPredicateExpressionNode : LongCountAsyncExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, CancellationToken, Task<long>>(SDataQueryableExtensions.LongCountAsync).GetMethodInfo().GetGenericMethodDefinition()
            };

        public LongCountAsyncWithoutPredicateExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression optionalCancel)
            : base(parseInfo, null, optionalCancel)
        {
        }
    }
}