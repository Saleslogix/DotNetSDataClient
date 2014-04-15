// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses.Expressions;

namespace Saleslogix.SData.Client.Linq
{
    internal static class MemberPathBuilder
    {
        public static IEnumerable<MemberInfo> Build(Expression expression, bool throwOnError = true)
        {
            var memberPath = new List<MemberInfo>();

            var expr = expression;
            while (!(expr is QuerySourceReferenceExpression ||
                     expr is ParameterExpression))
            {
                var memberExpr = expr as MemberExpression;
                if (memberExpr == null)
                {
                    if (throwOnError)
                    {
                        throw new InvalidOperationException("Paths must only include member expressions");
                    }
                    return null;
                }

                memberPath.Add(memberExpr.Member);
                expr = memberExpr.Expression;
            }

            memberPath.Reverse();
            return memberPath;
        }
    }
}