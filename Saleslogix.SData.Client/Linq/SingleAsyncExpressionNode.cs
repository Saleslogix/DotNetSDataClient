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
    internal class SingleAsyncExpressionNode : SingleExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<object>>(SDataQueryableExtensions.SingleAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, CancellationToken, Task<object>>(SDataQueryableExtensions.SingleOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public SingleAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression predicate, ConstantExpression optionalCancel)
            : base(parseInfo, predicate)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new SingleAsyncResultOperator(ParsedExpression.Method.Name.EndsWith("OrDefaultAsync"), _cancel);
        }
    }

    internal class SingleAsyncWithoutPredicateExpressionNode : SingleAsyncExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, CancellationToken, Task<object>>(SDataQueryableExtensions.SingleAsync).GetMethodInfo().GetGenericMethodDefinition(),
                new Func<IQueryable<object>, CancellationToken, Task<object>>(SDataQueryableExtensions.SingleOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
            };

        public SingleAsyncWithoutPredicateExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression optionalCancel)
            : base(parseInfo, null, optionalCancel)
        {
        }
    }
}