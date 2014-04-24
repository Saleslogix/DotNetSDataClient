// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class FetchExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, Expression<Func<object, object>>, IQueryable<object>>(SDataQueryableExtensions.Fetch).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly ResolvedExpressionCache<Expression> _cachedSelector;

        public FetchExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression selector)
            : base(parseInfo, null, null)
        {
            ArgumentUtility.CheckNotNull("selector", selector);

            if (selector.Parameters.Count != 1)
            {
                throw new ArgumentException("Selector must have exactly one parameter", "selector");
            }

            Selector = selector;
            _cachedSelector = new ResolvedExpressionCache<Expression>(this);
        }

        public LambdaExpression Selector { get; private set; }

        public Expression GetResolvedSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedSelector.GetOrCreate(r => r.GetResolvedExpression(Selector.Body, Selector.Parameters[0], clauseGenerationContext));
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new FetchResultOperator(GetResolvedSelector(clauseGenerationContext));
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            ArgumentUtility.CheckNotNull("inputParameter", inputParameter);
            ArgumentUtility.CheckNotNull("expressionToBeResolved", expressionToBeResolved);
            ArgumentUtility.CheckNotNull("clauseGenerationContext", clauseGenerationContext);
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }
    }
}