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
    internal class CountAsyncExpressionNode : CountExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, Task<int>>(SDataQueryableExtensions.CountAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, Task<int>>(SDataQueryableExtensions.CountAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        public CountAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
            : base(parseInfo, optionalPredicate)
        {
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new CountAsyncResultOperator();
        }
    }
}