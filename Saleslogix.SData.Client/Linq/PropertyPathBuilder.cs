// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;

namespace Saleslogix.SData.Client.Linq
{
    internal static class PropertyPathBuilder
    {
        public static IEnumerable<object> Build(Expression expression, bool throwOnError = true)
        {
            List<object> propPath = null;

            var expr = expression;
            while (!(expr is QuerySourceReferenceExpression || expr is ParameterExpression))
            {
                object prop = null;

                var memberExpr = expr as MemberExpression;
                if (memberExpr != null)
                {
                    prop = memberExpr.Member;
                    expr = memberExpr.Expression;
                }
                else
                {
                    var callExpr = expr as MethodCallExpression;
                    if (callExpr != null)
                    {
                        var method = callExpr.Method;
                        if (method.IsSpecialName && method.Name == "get_Item" && callExpr.Arguments.Count == 1)
                        {
                            var arg = callExpr.Arguments[0];
                            if (arg.NodeType == ExpressionType.Constant && arg.Type == typeof (string))
                            {
                                prop = ((ConstantExpression) arg).Value;
                                expr = callExpr.Object;
                            }
                        }
                    }
                    else
                    {
                        var unaryExpr = expr as UnaryExpression;
                        if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                        {
                            expr = unaryExpr.Operand;
                            continue;
                        }
                    }
                }

                if (prop == null)
                {
                    propPath = null;
                    break;
                }

                if (propPath == null)
                {
                    propPath = new List<object>();
                }
                propPath.Add(prop);
            }

            if (propPath == null)
            {
                if (throwOnError)
                {
                    throw new InvalidOperationException("Property paths must only include member and indexer expressions");
                }
                return null;
            }

            propPath.Reverse();
            return propPath;
        }
    }
}