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
    internal class AnyAsyncExpressionNode : AnyExpressionNode
    {
        public new static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, Task<bool>>(SDataQueryableExtensions.AnyAsync).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, Task<bool>>(SDataQueryableExtensions.AnyAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        public AnyAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
            : base(parseInfo, optionalPredicate)
        {
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new AnyAsyncResultOperator();
        }
    }
}