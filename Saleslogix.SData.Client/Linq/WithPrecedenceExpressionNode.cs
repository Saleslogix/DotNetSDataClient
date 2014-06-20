// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    internal class WithPrecedenceExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
                {
                    new Func<IQueryable<object>, int, IQueryable<object>>(SDataQueryableExtensions.WithPrecedence).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly int _precedence;

        public WithPrecedenceExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression value)
            : base(parseInfo, null, null)
        {
            _precedence = (int) value.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new WithPrecedenceResultOperator(_precedence);
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