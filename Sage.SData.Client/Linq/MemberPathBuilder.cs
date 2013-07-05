using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses.Expressions;

namespace Sage.SData.Client.Linq
{
    internal static class MemberPathBuilder
    {
        public static IEnumerable<MemberInfo> Build(Expression expression)
        {
            var memberPath = new List<MemberInfo>();

            var expr = expression;
            while (!(expr is QuerySourceReferenceExpression ||
                     expr is ParameterExpression))
            {
                var memberExpr = expr as MemberExpression;
                if (memberExpr == null)
                {
                    throw new NotSupportedException();
                }

                memberPath.Add(memberExpr.Member);
                expr = memberExpr.Expression;
            }

            memberPath.Reverse();
            return memberPath;
        }
    }
}