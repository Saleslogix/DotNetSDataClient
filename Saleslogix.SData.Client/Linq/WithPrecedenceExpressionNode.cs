using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Sage.SData.Client.Linq
{
    internal class WithPrecedenceExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
            new[]
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
            throw new NotSupportedException();
        }
    }
}