// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Saleslogix.SData.Client.Linq
{
    internal class ToCollectionAsyncExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IQueryable<object>, Task<ICollection<object>>>(SDataQueryableExtensions.ToCollectionAsync).GetMethodInfo().GetGenericMethodDefinition()
                };

        public ToCollectionAsyncExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
            : base(parseInfo, optionalPredicate, null)
        {
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw CreateResolveNotSupportedException();
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new ToCollectionAsyncResultOperator();
        }
    }
}