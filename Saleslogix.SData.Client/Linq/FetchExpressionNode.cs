using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Sage.SData.Client.Linq
{
    internal class FetchExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, Expression<Func<object, object>>, IQueryable<object>>(SDataQueryableExtensions.Fetch).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly Expression _selector;

        public FetchExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression selector)
            : base(parseInfo, null, null)
        {
            _selector = selector.Body;
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new FetchResultOperator(_selector);
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw new NotSupportedException();
        }
    }
}