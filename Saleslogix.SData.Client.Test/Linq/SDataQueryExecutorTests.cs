using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Saleslogix.SData.Client.Linq;
using Saleslogix.SData.Client.Test.Model;

#if !NET_3_5
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client.Test.Linq
{
    [TestFixture]
    public class SDataQueryExecutorTests
    {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
        [Test]
        public void Scalar_All_Same_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 3), CreateCollection<object>(null, 3));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new AllResultOperator(Expression.Constant(true)));
            var result = executor.ExecuteScalar<bool>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Where, Is.EqualTo("true"));
            Assert.That(result, Is.True);
        }

        [Test]
        public void Scalar_All_Different_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 3), CreateCollection<object>(null, 2));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new AllResultOperator(Expression.Constant(true)));
            var result = executor.ExecuteScalar<bool>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Where, Is.EqualTo("true"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void Scalar_Any_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new AnyResultOperator());
            var result = executor.ExecuteScalar<bool>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.True);
        }

        [Test]
        public void Scalar_Any_Take_Zero_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient<object>(parmsList);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(0)));
            builder.AddResultOperator(new AnyResultOperator());
            var result = executor.ExecuteScalar<bool>(builder.Build());

            Assert.That(parmsList, Is.Empty);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Scalar_Count_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new CountResultOperator());
            var result = executor.ExecuteScalar<int>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Scalar_Count_Take_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(3)));
            builder.AddResultOperator(new CountResultOperator());
            var result = executor.ExecuteScalar<int>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void Scalar_Count_Skip_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(3)));
            builder.AddResultOperator(new CountResultOperator());
            var result = executor.ExecuteScalar<int>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void Scalar_LongCount_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new LongCountResultOperator());
            var result = executor.ExecuteScalar<long>(builder.Build());

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Scalar_ExtensionArgs_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new WithExtensionArgResultOperator("foo", "bar"));
            builder.AddResultOperator(new CountResultOperator());
            executor.ExecuteScalar<int>(builder.Build());

            Assert.That(parmsList[0].ExtensionArgs, Is.EqualTo(new Dictionary<string, string> {{"foo", "bar"}}));
        }

        [Test]
        public void Single_First_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new FirstResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_First_Take_Zero_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient<Contact>(parmsList);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(0)));
            builder.AddResultOperator(new FirstResultOperator(true));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList, Is.Empty);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Single_Last_Unordered_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var clientMock = new Mock<ISDataClient>();
            var requestMock = new Mock<SDataParameters>();
            requestMock.SetupAllProperties();
            var responseMock1 = new Mock<ISDataResults<SDataCollection<object>>>();
            responseMock1.Setup(x => x.Content).Returns(CreateCollection<object>(null, 10));
            clientMock.Setup(x => x.Execute<SDataCollection<object>>(It.IsAny<SDataParameters>()))
                      .Returns(responseMock1.Object)
                      .Callback((SDataParameters parms) => parmsList.Add(parms));
            var responseMock2 = new Mock<ISDataResults<SDataCollection<Contact>>>();
            responseMock2.Setup(x => x.Content).Returns(CreateCollection(new[] {expected}, 10));
            clientMock.Setup(x => x.Execute<SDataCollection<Contact>>(It.IsAny<SDataParameters>()))
                      .Returns(responseMock2.Object)
                      .Callback((SDataParameters parms) => parmsList.Add(parms));
            var executor = new SDataQueryExecutor(clientMock.Object);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new LastResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Count, Is.EqualTo(1));
            Assert.That(parmsList[1].StartIndex, Is.EqualTo(10));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_Last_Unordered_Empty_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClient(parmsList, CreateCollection<object>(null, 0));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new LastResultOperator(true));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Single_Last_Ordered_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(((Expression<Func<Contact, string>>) (c => c.FirstName)).Body, OrderingDirection.Asc)
                                          }
                                  });
            builder.AddResultOperator(new LastResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(parmsList[0].OrderBy, Is.EqualTo("FirstName desc"));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_Single_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 1));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SingleResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_Single_TotalResultsMissing_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, null));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SingleResultOperator(false));
            Assert.That(() => executor.ExecuteSingle<Contact>(builder.Build(), false), Throws.InstanceOf<SDataClientException>());
        }

        [Test]
        public void Single_Single_MultipleResults_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 2));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SingleResultOperator(false));
            Assert.That(() => executor.ExecuteSingle<Contact>(builder.Build(), false), Throws.InstanceOf<SDataClientException>());
        }

        [Test]
        public void Single_Single_Take_One_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(1)));
            builder.AddResultOperator(new SingleResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_Single_Skip_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(9)));
            builder.AddResultOperator(new SingleResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_Single_Take_Skip_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(5)));
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(4)));
            builder.AddResultOperator(new SingleResultOperator(false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Single_ElementAt_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClient(parmsList, CreateCollection(new[] {expected}, 1));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new ElementAtResultOperator(3, false));
            var result = executor.ExecuteSingle<Contact>(builder.Build(), false);

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(parmsList[0].StartIndex, Is.EqualTo(4));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Collection_Test()
        {
            var expected = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 3);
            var client = CreateClient(null, expected);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            var result = executor.ExecuteCollection<Contact>(builder.Build());

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void Collection_Paging_Test()
        {
            var page1 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var page2 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var client = CreateClient(null, page1, page2);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            var result = executor.ExecuteCollection<Contact>(builder.Build());

            Assert.That(result, Is.EquivalentTo(page1.Concat(page2)));
        }

        [Test]
        public void Collection_Select_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new[] {"one", "two", "three"};
            var list = CreateCollection(expected.Select(lastName => new Contact {LastName = lastName}), 3);
            var client = CreateClient(parmsList, list);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(false);
            builder.AddClause(new SelectClause(
                                  Expression.Property(
                                      new QuerySourceReferenceExpression(builder.MainFromClause),
                                      "LastName")));
            var result = executor.ExecuteCollection<string>(builder.Build());

            Assert.That(result, Is.EquivalentTo(expected));
            Assert.That(parmsList[0].Select, Is.EqualTo("LastName"));
        }

        [Test]
        public void Collection_Take_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 10);
            var client = CreateClient(parmsList, expected);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(3)));
            var result = executor.ExecuteCollection<Contact>(builder.Build());

            Assert.That(result, Is.EquivalentTo(expected));
            Assert.That(parmsList[0].Count, Is.EqualTo(3));
        }

        [Test]
        public void Collection_Take_Large_Test()
        {
            var parmsList = new List<SDataParameters>();
            var page1 = CreateCollection(Enumerable.Range(0, 100).Select(i => new Contact()), 200);
            var page2 = CreateCollection(Enumerable.Range(0, 100).Select(i => new Contact()), 200);
            var client = CreateClient(parmsList, page1, page2);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(150)));
            var result = executor.ExecuteCollection<Contact>(builder.Build());

            Assert.That(result, Is.EquivalentTo(page1.Concat(page2.Take(50))));
            Assert.That(parmsList[1].Count, Is.EqualTo(50));
        }

        [Test]
        public void Collection_WithoutTotalResults_Test()
        {
            var expected = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), null);
            var client = CreateClient(null, expected, CreateCollection<Contact>(null, null));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            var result = executor.ExecuteCollection<Contact>(builder.Build());

            Assert.That(result, Is.EquivalentTo(expected));
        }
#endif

#if !NET_3_5
        [Test]
        public void ScalarAsync_All_Same_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 3), CreateCollection<object>(null, 3));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new AllAsyncResultOperator(Expression.Constant(true), CancellationToken.None));
            var result = executor.ExecuteScalarAsync<bool>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Where, Is.EqualTo("true"));
            Assert.That(result, Is.True);
        }

        [Test]
        public void ScalarAsync_All_Different_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 3), CreateCollection<object>(null, 2));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new AllAsyncResultOperator(Expression.Constant(true), CancellationToken.None));
            var result = executor.ExecuteScalarAsync<bool>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Where, Is.EqualTo("true"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void ScalarAsync_Any_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new AnyAsyncResultOperator(CancellationToken.None));
            var result = executor.ExecuteScalarAsync<bool>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.True);
        }

        [Test]
        public void ScalarAsync_Any_Take_Zero_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(0)));
            builder.AddResultOperator(new AnyAsyncResultOperator(CancellationToken.None));
            var result = executor.ExecuteScalarAsync<bool>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList, Is.Empty);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ScalarAsync_Count_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new CountAsyncResultOperator(CancellationToken.None));
            var result = executor.ExecuteScalarAsync<int>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void ScalarAsync_Count_Take_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(3)));
            builder.AddResultOperator(new CountAsyncResultOperator(CancellationToken.None));
            var result = executor.ExecuteScalarAsync<int>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ScalarAsync_Count_Skip_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SkipResultOperator(Expression.Constant(3)));
            builder.AddResultOperator(new CountAsyncResultOperator(CancellationToken.None));
            var result = executor.ExecuteScalarAsync<int>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void ScalarAsync_LongCount_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new LongCountAsyncResultOperator(CancellationToken.None));
            var result = executor.ExecuteScalarAsync<long>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void ScalarAsync_ExtensionArgs_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new WithExtensionArgResultOperator("foo", "bar"));
            builder.AddResultOperator(new CountAsyncResultOperator(CancellationToken.None));
            executor.ExecuteScalarAsync<int>(builder.Build(), CancellationToken.None).Wait();

            Assert.That(parmsList[0].ExtensionArgs, Is.EqualTo(new Dictionary<string, string> {{"foo", "bar"}}));
        }

        [Test]
        public void SingleAsync_First_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClientAsync(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new FirstAsyncResultOperator(false, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SingleAsync_First_Take_Zero_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync<Contact>(parmsList);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(0)));
            builder.AddResultOperator(new FirstAsyncResultOperator(true, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList, Is.Empty);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void SingleAsync_Last_Unordered_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var clientMock = new Mock<ISDataClient>();
            var requestMock = new Mock<SDataParameters>();
            requestMock.SetupAllProperties();
            var responseMock1 = new Mock<ISDataResults<SDataCollection<object>>>();
            responseMock1.Setup(x => x.Content).Returns(CreateCollection<object>(null, 10));
            clientMock.Setup(x => x.ExecuteAsync<SDataCollection<object>>(It.IsAny<SDataParameters>(), CancellationToken.None))
                      .Returns(() =>
                                   {
                                       var taskSource = new TaskCompletionSource<ISDataResults<SDataCollection<object>>>();
                                       taskSource.SetResult(responseMock1.Object);
                                       return taskSource.Task;
                                   })
                      .Callback((SDataParameters parms, CancellationToken cancel) => parmsList.Add(parms));
            var responseMock2 = new Mock<ISDataResults<SDataCollection<Contact>>>();
            responseMock2.Setup(x => x.Content).Returns(CreateCollection(new[] {expected}, 10));
            clientMock.Setup(x => x.ExecuteAsync<SDataCollection<Contact>>(It.IsAny<SDataParameters>(), CancellationToken.None))
                      .Returns(() =>
                                   {
                                       var taskSource = new TaskCompletionSource<ISDataResults<SDataCollection<Contact>>>();
                                       taskSource.SetResult(responseMock2.Object);
                                       return taskSource.Task;
                                   })
                      .Callback((SDataParameters parms, CancellationToken cancel) => parmsList.Add(parms));
            var executor = new SDataQueryExecutor(clientMock.Object);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new LastAsyncResultOperator(false, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(parmsList[1].Count, Is.EqualTo(1));
            Assert.That(parmsList[1].StartIndex, Is.EqualTo(10));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SingleAsync_Last_Unordered_Empty_Test()
        {
            var parmsList = new List<SDataParameters>();
            var client = CreateClientAsync(parmsList, CreateCollection<object>(null, 0));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new LastAsyncResultOperator(true, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(0));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void SingleAsync_Last_Ordered_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClientAsync(parmsList, CreateCollection(new[] {expected}, 10));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddClause(new OrderByClause
                                  {
                                      Orderings =
                                          {
                                              new Ordering(((Expression<Func<Contact, string>>) (c => c.FirstName)).Body, OrderingDirection.Asc)
                                          }
                                  });
            builder.AddResultOperator(new LastAsyncResultOperator(false, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(parmsList[0].OrderBy, Is.EqualTo("FirstName desc"));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SingleAsync_Single_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClientAsync(parmsList, CreateCollection(new[] {expected}, 1));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new SingleAsyncResultOperator(false, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SingleAsync_ElementAt_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new Contact();
            var client = CreateClientAsync(parmsList, CreateCollection(new[] {expected}, 1));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new ElementAtAsyncResultOperator(3, false, CancellationToken.None));
            var result = executor.ExecuteSingleAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(parmsList[0].Count, Is.EqualTo(1));
            Assert.That(parmsList[0].StartIndex, Is.EqualTo(4));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CollectionAsync_Test()
        {
            var expected = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 3);
            var client = CreateClientAsync(null, expected);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            var result = executor.ExecuteCollectionAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void CollectionAsync_Paging_Test()
        {
            var page1 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var page2 = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 6);
            var client = CreateClientAsync(null, page1, page2);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            var result = executor.ExecuteCollectionAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(result, Is.EquivalentTo(page1.Concat(page2)));
        }

        [Test]
        public void CollectionAsync_Select_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = new[] {"one", "two", "three"};
            var list = CreateCollection(expected.Select(lastName => new Contact {LastName = lastName}), 3);
            var client = CreateClientAsync(parmsList, list);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(false);
            builder.AddClause(new SelectClause(
                                  Expression.Property(
                                      new QuerySourceReferenceExpression(builder.MainFromClause),
                                      "LastName")));
            var result = executor.ExecuteCollectionAsync<string>(builder.Build(), CancellationToken.None).Result;

            Assert.That(result, Is.EquivalentTo(expected));
            Assert.That(parmsList[0].Select, Is.EqualTo("LastName"));
        }

        [Test]
        public void CollectionAsync_Take_Test()
        {
            var parmsList = new List<SDataParameters>();
            var expected = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), 10);
            var client = CreateClientAsync(parmsList, expected);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(3)));
            var result = executor.ExecuteCollectionAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(result, Is.EquivalentTo(expected));
            Assert.That(parmsList[0].Count, Is.EqualTo(3));
        }

        [Test]
        public void CollectionAsync_Take_Large_Test()
        {
            var parmsList = new List<SDataParameters>();
            var page1 = CreateCollection(Enumerable.Range(0, 100).Select(i => new Contact()), 200);
            var page2 = CreateCollection(Enumerable.Range(0, 100).Select(i => new Contact()), 200);
            var client = CreateClientAsync(parmsList, page1, page2);
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            builder.AddResultOperator(new TakeResultOperator(Expression.Constant(150)));
            var result = executor.ExecuteCollectionAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(result, Is.EquivalentTo(page1.Concat(page2.Take(50))));
            Assert.That(parmsList[1].Count, Is.EqualTo(50));
        }

        [Test]
        public void CollectionAsync_WithoutTotalResults_Test()
        {
            var expected = CreateCollection(Enumerable.Range(0, 3).Select(i => new Contact()), null);
            var client = CreateClientAsync(null, expected, CreateCollection<Contact>(null, null));
            var executor = new SDataQueryExecutor(client);
            var builder = CreateQueryBuilder<Contact>(true);
            var result = executor.ExecuteCollectionAsync<Contact>(builder.Build(), CancellationToken.None).Result;

            Assert.That(result, Is.EquivalentTo(expected));
        }
#endif

        private static SDataCollection<T> CreateCollection<T>(IEnumerable<T> collection, int? totalResults)
        {
            var list = new SDataCollection<T>();

            if (collection != null)
            {
                list.AddRange(collection);
            }

            if (totalResults != null)
            {
                ((ISDataProtocolObject) list).Info = new SDataProtocolInfo {TotalResults = totalResults};
            }

            return list;
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        private static ISDataClient CreateClient<T>(ICollection<SDataParameters> parmsList, params T[] responseDataSequence)
        {
            var clientMock = new Mock<ISDataClient>();
            var responses = new Queue<ISDataResults<T>>(
                responseDataSequence.Select(
                    data =>
                        {
                            var responseMock = new Mock<ISDataResults<T>>();
                            responseMock.Setup(x => x.Content).Returns(data);
                            return responseMock.Object;
                        }));
            clientMock.Setup(x => x.Execute<T>(It.IsAny<SDataParameters>()))
                      .Returns(responses.Dequeue)
                      .Callback((SDataParameters parms) =>
                                    {
                                        if (parmsList != null)
                                        {
                                            parmsList.Add(parms);
                                        }
                                    });
            return clientMock.Object;
        }
#endif

#if !NET_3_5
        private static ISDataClient CreateClientAsync<T>(ICollection<SDataParameters> parmsList, params T[] responseDataSequence)
        {
            var clientMock = new Mock<ISDataClient>();
            var responses = new Queue<Task<ISDataResults<T>>>(
                responseDataSequence.Select(
                    data =>
                        {
                            var responseMock = new Mock<ISDataResults<T>>();
                            responseMock.Setup(x => x.Content).Returns(data);
                            var taskSource = new TaskCompletionSource<ISDataResults<T>>();
                            taskSource.SetResult(responseMock.Object);
                            return taskSource.Task;
                        }));
            clientMock.Setup(x => x.ExecuteAsync<T>(It.IsAny<SDataParameters>(), CancellationToken.None))
                      .Returns(responses.Dequeue)
                      .Callback((SDataParameters parms, CancellationToken cancel) =>
                                    {
                                        if (parmsList != null)
                                        {
                                            parmsList.Add(parms);
                                        }
                                    });
            return clientMock.Object;
        }
#endif

        private static QueryModelBuilder CreateQueryBuilder<T>(bool attachSelect)
        {
            var builder = new QueryModelBuilder();
            builder.AddClause(new MainFromClause("x", typeof (T), Expression.Constant(null)));

            if (attachSelect)
            {
                builder.AddClause(new SelectClause(new QuerySourceReferenceExpression(builder.MainFromClause)));
            }

            return builder;
        }
    }
}