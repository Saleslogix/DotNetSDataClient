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
    internal class LastAsyncExpressionNode : LastExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<object>>(SDataQueryableExtensions.LastAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<object>>(SDataQueryableExtensions.LastOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public LastAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate, ConstantExpression optionalCancel)
            : base(parseInfo, predicate)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new LastAsyncResultOperator(ParsedExpression.Method.Name.EndsWith("OrDefaultAsync"), _cancel);
        }
    }

    internal class LastAsyncWithoutPredicateExpressionNode : LastAsyncExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, CancellationToken, Task<object>>(SDataQueryableExtensions.LastAsync).GetMethodInfo().GetGenericMethodDefinition(),
                new Func<IQueryable<object>, CancellationToken, Task<object>>(SDataQueryableExtensions.LastOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
            };

        public LastAsyncWithoutPredicateExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression optionalCancel)
            : base(parseInfo, null, optionalCancel)
        {
        }
    }
}