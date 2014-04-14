using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Saleslogix.SData.Client.Linq;
using Saleslogix.SData.Client.Test.Model;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Linq
{
    [TestFixture]
    public class SDataQueryModelVisitorTests
    {
        [Test]
        public void MainType_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.MainType, Is.EqualTo(typeof (Contact)));
        }

        [Test]
        public void Where_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new WhereClause(GetExpression((Contact c) => c.Active)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Where, Is.EqualTo("Active"));
        }

        [Test]
        public void Where_Multiple_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new WhereClause(GetExpression((Contact c) => c.Active)));
            builder.AddClause(new WhereClause(GetExpression((Contact c) => c.Active)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Where, Is.EqualTo("(Active and Active)"));
        }

        [Test]
        public void OrderBy_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(GetExpression((Contact c) => c.FirstName), OrderingDirection.Asc)
                                          }
                                  });
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.OrderBy, Is.EqualTo("FirstName asc"));
        }

        [Test]
        public void OrderBy_Nested_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(GetExpression((Contact c) => c.Address.PostalCode), OrderingDirection.Asc)
                                          }
                                  });
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.OrderBy, Is.EqualTo("Address.PostalCode asc"));
        }

        [Test]
        public void OrderBy_Duplicate_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(GetExpression((Contact c) => c.FirstName), OrderingDirection.Asc)
                                          }
                                  });
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(GetExpression((Contact c) => c.LastName), OrderingDirection.Desc)
                                          }
                                  });
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.OrderBy, Is.EqualTo("LastName desc,FirstName asc"));
        }

        [Test]
        public void OrderBy_Multiple_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(GetExpression((Contact c) => c.FirstName), OrderingDirection.Asc),
                                              new Ordering(GetExpression((Contact c) => c.LastName), OrderingDirection.Desc)
                                          }
                                  });
            builder.AddClause(new SelectClause(Expression.Constant(null)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.OrderBy, Is.EqualTo("FirstName asc,LastName desc"));
        }

        [Test]
        public void Select_Single_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(GetExpression((Contact c) => c.FirstName)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Select, Is.EqualTo("FirstName"));
        }

        [Test]
        public void Select_Anonymous_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(GetExpression((Contact c) => new {c.FirstName, c.LastName})));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Select, Is.EqualTo("FirstName,LastName"));
        }

        [Test]
        public void Select_Array_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(GetExpression((Contact c) => new[] {c.FirstName, c.LastName})));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Select, Is.EqualTo("FirstName,LastName"));
        }

        [Test]
        public void Select_Nested_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(GetExpression((Contact c) => c.Address.PostalCode)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Select, Is.EqualTo("Address/PostalCode"));
        }

        [Test]
        public void ResultOperator_Take_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(10)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Count, Is.EqualTo(10));
        }

        [Test]
        public void ResultOperator_Take_Multiple_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(10)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(5)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(8)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Count, Is.EqualTo(5));
        }

        [Test]
        public void ResultOperator_Skip_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(10)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.StartIndex, Is.EqualTo(11));
        }

        [Test]
        public void ResultOperator_Skip_Multiple_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(10)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(10)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(10)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.StartIndex, Is.EqualTo(31));
        }

        [Test]
        public void ResultOperator_Skip_Take_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(10)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(3)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.StartIndex, Is.EqualTo(11));
            Assert.That(visitor.Count, Is.EqualTo(3));
        }

        [Test]
        public void ResultOperator_Take_Skip_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(10)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(3)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.StartIndex, Is.EqualTo(4));
            Assert.That(visitor.Count, Is.EqualTo(7));
        }

        [Test]
        public void ResultOperator_Fetch_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new FetchResultOperator(GetExpression((Contact c) => c.Address)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Include, Is.EqualTo("Address"));
        }

        [Test]
        public void ResultOperator_Fetch_Multiple_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new FetchResultOperator(GetExpression((Contact c) => c.Address)));
            builder.AddResultOperator(new FetchResultOperator(GetExpression((Contact c) => c.Address.Street)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Include, Is.EqualTo("Address,Address/Street"));
        }

        [Test]
        public void ResultOperator_WithPrecedence_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new WithPrecedenceResultOperator(3));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Precedence, Is.EqualTo(3));
        }

        [Test]
        public void ResultOperator_WithExtensionArg_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddResultOperator(new WithExtensionArgResultOperator("foo", "bar"));
            builder.AddResultOperator(new WithExtensionArgResultOperator("hello", "world"));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.ExtensionArgs, Is.EqualTo(new Dictionary<string, string> {{"foo", "bar"}, {"hello", "world"}}));
        }

        [Test]
        public void SubQuery_Test()
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), Expression.Constant(null)));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddClause(new WhereClause(GetExpression((Contact c) => c.Active)));
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(10)));

            var subQuery = new SubQueryExpression(builder.Build());
            builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (Contact), subQuery));
            builder.AddClause(new SelectClause(Expression.Constant(null)));
            builder.AddClause(new WhereClause(GetExpression((Contact c) => c.Active)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(3)));

            var visitor = new SDataQueryModelVisitor();
            visitor.VisitQueryModel(builder.Build());

            Assert.That(visitor.Where, Is.EqualTo("(Active and Active)"));
            Assert.That(visitor.StartIndex, Is.EqualTo(4));
            Assert.That(visitor.Count, Is.EqualTo(7));
        }

        private static Expression GetExpression<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return expression.Body;
        }
    }
}