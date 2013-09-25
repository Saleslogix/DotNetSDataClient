// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Saleslogix.SData.Client.Linq
{
    internal class LastAsyncExpressionNode : LastExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, Task<object>>(SDataQueryableExtensions.LastAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Task<object>>(SDataQueryableExtensions.LastOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, Task<object>>(SDataQueryableExtensions.LastAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, Task<object>>(SDataQueryableExtensions.LastOrDefaultAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        public LastAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
            : base(parseInfo, optionalPredicate)
        {
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new LastAsyncResultOperator(ParsedExpression.Method.Name.EndsWith("OrDefaultAsync"));
        }
    }
}