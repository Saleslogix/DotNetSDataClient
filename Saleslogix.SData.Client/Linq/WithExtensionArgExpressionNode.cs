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
    internal class WithExtensionArgExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
            {
                new Func<IQueryable<object>, string, string, IQueryable<object>>(SDataQueryableExtensions.WithExtensionArg).GetMethodInfo().GetGenericMethodDefinition()
            };

        private readonly string _name;
        private readonly string _value;

        public WithExtensionArgExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression name, ConstantExpression value)
            : base(parseInfo, null, null)
        {
            _name = (string) name.Value;
            _value = (string) value.Value;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new WithExtensionArgResultOperator(_name, _value);
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