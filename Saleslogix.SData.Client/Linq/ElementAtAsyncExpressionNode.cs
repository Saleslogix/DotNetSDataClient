// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Saleslogix.SData.Client.Linq
{
    internal class ElementAtAsyncExpressionNode : ElementAtExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, int, CancellationToken, object>(SDataQueryableExtensions.ElementAtAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, int, CancellationToken, object>(SDataQueryableExtensions.ElementAtOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly CancellationToken _cancel;

        public ElementAtAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression index, ConstantExpression optionalCancel)
            : base(parseInfo, index)
        {
            _cancel = (CancellationToken) optionalCancel.Value;
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw CreateResolveNotSupportedException();
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new ElementAtAsyncResultOperator(Index, ParsedExpression.Method.Name.EndsWith("OrDefaultAsync"), _cancel);
        }
    }
}