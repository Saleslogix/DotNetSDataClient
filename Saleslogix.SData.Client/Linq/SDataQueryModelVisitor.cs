// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using Remotion.Linq.Parsing;

namespace Saleslogix.SData.Client.Linq
{
    internal class SDataQueryModelVisitor : QueryModelVisitorBase
    {
        private readonly IDictionary<string, string> _extensionArgs = new Dictionary<string, string>();
        private readonly INamingScheme _namingScheme;

        public SDataQueryModelVisitor(INamingScheme namingScheme = null)
        {
            _namingScheme = namingScheme ?? NamingScheme.Default;
        }

        public Type MainType { get; private set; }
        public string Select { get; private set; }
        public string Where { get; private set; }
        public string OrderBy { get; private set; }
        public int? Count { get; private set; }
        public int? StartIndex { get; private set; }
        public string Include { get; private set; }
        public int? Precedence { get; private set; }

        public IDictionary<string, string> ExtensionArgs
        {
            get { return _extensionArgs; }
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            MainType = fromClause.ItemType;

            var subQueryExpr = fromClause.FromExpression as SubQueryExpression;
            if (subQueryExpr != null)
            {
                VisitQueryModel(subQueryExpr.QueryModel);
            }
            else
            {
                var constExpr = fromClause.FromExpression as ConstantExpression;
                if (constExpr == null || (constExpr.Value != null && !(constExpr.Value is IQueryable)))
                {
                    throw new NotSupportedException("From expression not supported");
                }
            }

            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            var where = SDataExpressionBuilderVisitor.BuildExpression(whereClause.Predicate, _namingScheme);
            if (Where != null)
            {
                where = string.Format("({0} and {1})", Where, where);
            }

            Where = where;
            base.VisitWhereClause(whereClause, queryModel, index);
        }

        protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
        {
            if (OrderBy != null)
            {
                OrderBy = "," + OrderBy;
            }

            OrderBy = string.Join(",", orderings.Select(
                ordering => string.Format(
                    "{0} {1}",
                    RenderPropertyPath(PropertyPathBuilder.Build(ordering.Expression, true), ".", _namingScheme),
                    ordering.OrderingDirection.ToString().ToLowerInvariant())).ToArray()) + OrderBy;
            base.VisitOrderings(orderings, queryModel, orderByClause);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            var paths = PropertyPathExtractionExpressionTreeVisitor.ExtractPropertyPaths(selectClause.Selector, _namingScheme);
            if (paths.Count > 0)
            {
                Select = string.Join(",", paths.ToArray());
            }
            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (resultOperator is AnyResultOperator ||
                resultOperator is CountResultOperator ||
                resultOperator is LongCountResultOperator ||
                resultOperator is FirstResultOperator ||
                resultOperator is SingleResultOperator)
            {
                return;
            }

            var takeOperator = resultOperator as TakeResultOperator;
            if (takeOperator != null)
            {
                var count = takeOperator.GetConstantCount();
                if (Count != null)
                {
                    count = Math.Min(count, Count.Value);
                }

                Count = count;
                return;
            }

            var skipOperator = resultOperator as SkipResultOperator;
            if (skipOperator != null)
            {
                var startIndex = skipOperator.GetConstantCount();
                if (Count != null)
                {
                    Count = Math.Max(Count.Value - startIndex, 0);
                }

                StartIndex = (StartIndex ?? 1) + startIndex;
                return;
            }

            var fetchOperator = resultOperator as FetchResultOperator;
            if (fetchOperator != null)
            {
                var include = RenderPropertyPath(fetchOperator.PropertyPath, "/", _namingScheme);
                if (Include != null)
                {
                    include = string.Format("{0},{1}", Include, include);
                }

                Include = include;
                return;
            }

            var withPrecedenceOperator = resultOperator as WithPrecedenceResultOperator;
            if (withPrecedenceOperator != null)
            {
                if (Precedence != null)
                {
                    throw new NotSupportedException("Multiple WithPrecedence operators not supported");
                }

                Precedence = withPrecedenceOperator.Precedence;
                return;
            }

            var withExtensionArgOperator = resultOperator as WithExtensionArgResultOperator;
            if (withExtensionArgOperator != null)
            {
                ExtensionArgs[withExtensionArgOperator.Name] = withExtensionArgOperator.Value;
                return;
            }

#if !NET_3_5
            if (resultOperator is ToCollectionAsyncResultOperator)
            {
                return;
            }
#endif

            throw new NotSupportedException(string.Format("Result operator '{0}' not supported", resultOperator.GetType()));
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Additional from clauses not supported");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Join clauses not supported");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            throw new NotSupportedException("Join clauses not supported");
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Group join clauses not supported");
        }

        private static string RenderPropertyPath(IEnumerable<object> propertyPath, string separator, INamingScheme namingScheme)
        {
            return string.Join(separator,
                propertyPath.Select(prop =>
                {
                    var member = prop as MemberInfo;
                    return member != null ? namingScheme.GetName(member) : prop.ToString();
                }).ToArray());
        }

        #region Nested type: MemberPathExtractionVisitor

        private class PropertyPathExtractionExpressionTreeVisitor : ExpressionTreeVisitor
        {
            public static ICollection<string> ExtractPropertyPaths(Expression expression, INamingScheme namingScheme)
            {
                var visitor = new PropertyPathExtractionExpressionTreeVisitor(namingScheme);
                visitor.VisitExpression(expression);
                return visitor._paths;
            }

            private readonly INamingScheme _namingScheme;
            private readonly ICollection<string> _paths = new HashSet<string>();

            private PropertyPathExtractionExpressionTreeVisitor(INamingScheme namingScheme)
            {
                _namingScheme = namingScheme;
            }

            public override Expression VisitExpression(Expression expression)
            {
                var path = PropertyPathBuilder.Build(expression, false, false);
                if (path != null)
                {
                    _paths.Add(RenderPropertyPath(path, "/", _namingScheme));
                    return expression;
                }

                return base.VisitExpression(expression);
            }
        }

        #endregion
    }
}