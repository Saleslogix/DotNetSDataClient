// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Saleslogix.SData.Client.Linq
{
    internal class ElementAtExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods =
            new[]
                {
                    new Func<IEnumerable<object>, int, object>(Enumerable.ElementAt).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, int, object>(Queryable.ElementAt).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IEnumerable<object>, int, object>(Enumerable.ElementAtOrDefault).GetMethodInfo().GetGenericMethodDefinition(),
                    new Func<IQueryable<object>, int, object>(Queryable.ElementAtOrDefault).GetMethodInfo().GetGenericMethodDefinition()
                };

        private readonly int _index;

        public ElementAtExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression index)
            : base(parseInfo, null, null)
        {
            _index = (int) index.Value;
        }

        public int Index
        {
            get { return _index; }
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw CreateResolveNotSupportedException();
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new ElementAtResultOperator(_index, ParsedExpression.Method.Name.EndsWith("OrDefault"));
        }
    }
}