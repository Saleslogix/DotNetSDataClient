using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Sage.SData.Client.Linq
{
    internal class LongCountAsyncExpressionNode : LongCountExpressionNode
    {
        public static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, Task<long>>(SDataQueryableExtensions.LongCountAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, Task<long>>(SDataQueryableExtensions.LongCountAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        public LongCountAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
            : base(parseInfo, optionalPredicate)
        {
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new LongCountAsyncResultOperator();
        }
    }
}