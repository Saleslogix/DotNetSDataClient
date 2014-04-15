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
    internal class FirstAsyncExpressionNode : FirstExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<object>>(SDataQueryableExtensions.FirstAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<object>>(SDataQueryableExtensions.FirstOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public FirstAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate, ConstantExpression optionalCancel)
            : base(parseInfo, predicate)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new FirstAsyncResultOperator(ParsedExpression.Method.Name.EndsWith("OrDefaultAsync"), _cancel);
        }
    }

    internal class FirstAsyncWithoutPredicateExpressionNode : FirstAsyncExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, CancellationToken, Task<object>>(SDataQueryableExtensions.FirstAsync).GetMethodInfo().GetGenericMethodDefinition(),
                new Func<IQueryable<object>, CancellationToken, Task<object>>(SDataQueryableExtensions.FirstOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
            };

        public FirstAsyncWithoutPredicateExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression optionalCancel)
            : base(parseInfo, null, optionalCancel)
        {
        }
    }
}