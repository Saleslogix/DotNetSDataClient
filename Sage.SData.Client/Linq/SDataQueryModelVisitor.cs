using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Collections;
using Remotion.Linq.Parsing;

namespace Sage.SData.Client.Linq
{
    internal class SDataQueryModelVisitor : QueryModelVisitorBase
    {
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
                    string.Join(".", MemberPathBuilder.Build(ordering.Expression).Select(item => _namingScheme.GetName(item)).ToArray()),
                    ordering.OrderingDirection.ToString().ToLowerInvariant())).ToArray()) + OrderBy;
            base.VisitOrderings(orderings, queryModel, orderByClause);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            var paths = MemberPathExtractionVisitor.ExtractPropertyPaths(selectClause.Selector, _namingScheme);
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
                resultOperator is LongCountResultOperator)
            {
                return;
            }

            if (resultOperator is FirstResultOperator ||
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
                var include = string.Join("/", fetchOperator.MemberPath.Select(item => _namingScheme.GetName(item)).ToArray());
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
                    throw new NotSupportedException();
                }

                Precedence = withPrecedenceOperator.Precedence;
                return;
            }

            throw new NotSupportedException();
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

        #region Nested type: MemberPathExtractionVisitor

        private class MemberPathExtractionVisitor : ExpressionTreeVisitor
        {
            public static ICollection<string> ExtractPropertyPaths(Expression expression, INamingScheme namingScheme)
            {
                var visitor = new MemberPathExtractionVisitor(namingScheme);
                visitor.VisitExpression(expression);
                return visitor._paths;
            }

            private readonly INamingScheme _namingScheme;
            private readonly ICollection<string> _paths = new HashSet<string>();

            private MemberPathExtractionVisitor(INamingScheme namingScheme)
            {
                _namingScheme = namingScheme;
            }

            protected override Expression VisitMemberExpression(MemberExpression expression)
            {
                _paths.Add(string.Join("/", MemberPathBuilder.Build(expression).Select(item => _namingScheme.GetName(item)).ToArray()));
                return expression;
            }
        }

        #endregion
    }
}