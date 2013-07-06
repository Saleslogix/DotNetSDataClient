using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Sage.SData.Client.Linq
{
    internal class ElementAtAsyncExpressionNode : ElementAtExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, int, object>(SDataQueryableExtensions.ElementAtAsync).Method.GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, int, object>(SDataQueryableExtensions.ElementAtOrDefaultAsync).Method.GetGenericMethodDefinition()
                };

        public ElementAtAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression index)
            : base(parseInfo, index)
        {
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw CreateResolveNotSupportedException();
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new ElementAtAsyncResultOperator(Index, ParsedExpression.Method.Name.EndsWith("OrDefaultAsync"));
        }
    }
}