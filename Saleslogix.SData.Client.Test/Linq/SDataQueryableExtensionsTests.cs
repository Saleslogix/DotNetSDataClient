using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Saleslogix.SData.Client.Linq;
using Saleslogix.SData.Client.Test.Model;

namespace Saleslogix.SData.Client.Test.Linq
{
    [TestFixture]
    public class SDataQueryableExtensionsTests
    {
        [Test]
        public void WithPrecedence_Test()
        {
            var result = new Contact[0].AsQueryable().WithPrecedence(3);
            Assert.That(result.Expression.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].WithPrecedence(3)"));
        }

        [Test]
        public void WithExtensionArg_Test()
        {
            var result = new Contact[0].AsQueryable().WithExtensionArg("foo", "bar");
            Assert.That(result.Expression.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].WithExtensionArg(\"foo\", \"bar\")"));
        }

        [Test]
        public void Fetch_Test()
        {
            var result = new Contact[0].AsQueryable().Fetch(x => x.LastName);
            Assert.That(result.Expression.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].Fetch(x => x.LastName)"));
        }

#if !NET_3_5
        [Test]
        public void ToListAsync_Test()
        {
            var result = ExecuteCollectionAsync(contacts => contacts.ToListAsync());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ToCollectionAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ToArrayAsync_Test()
        {
            var result = ExecuteCollectionAsync(contacts => contacts.ToArrayAsync());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ToCollectionAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ToDictionaryAsync_Test()
        {
            var result = ExecuteCollectionAsync(contacts => contacts.ToDictionaryAsync(x => x));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ToCollectionAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ToDictionaryAsync_WithElementSelector_Test()
        {
            var result = ExecuteCollectionAsync(contacts => contacts.ToDictionaryAsync(x => x, x => x));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ToCollectionAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ToLookupAsync_Test()
        {
            var result = ExecuteCollectionAsync(contacts => contacts.ToLookupAsync(x => x));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ToCollectionAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ToLookupAsync_WithElementSelector_Test()
        {
            var result = ExecuteCollectionAsync(contacts => contacts.ToLookupAsync(x => x, x => x));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ToCollectionAsync(value(System.Threading.CancellationToken))"));
        }

        private static Expression ExecuteCollectionAsync(Func<IQueryable<Contact>, Task> func)
        {
            var providerMock = new Mock<IQueryProvider>();
            Expression result = null;
            var taskSource = new TaskCompletionSource<ICollection<Contact>>();
            taskSource.SetResult(new Contact[0]);
            providerMock.Setup(x => x.Execute<Task<ICollection<Contact>>>(It.IsAny<Expression>()))
                .Callback((Expression e) => result = e)
                .Returns(taskSource.Task);
            var queryableMock = new Mock<IQueryable<Contact>>();
            queryableMock.Setup(x => x.Provider).Returns(providerMock.Object);
            queryableMock.Setup(x => x.Expression).Returns(Expression.Constant((new Contact[0]).AsQueryable()));
            func(queryableMock.Object).Wait(CancellationToken.None);
            return result;
        }

        [Test]
        public void CountAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.CountAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].CountAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void CountAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.CountAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].CountAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void LongCountAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.LongCountAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].LongCountAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void LongCountAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.LongCountAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].LongCountAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void AnyAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.AnyAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].AnyAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void AnyAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.AnyAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].AnyAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void AllAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.AllAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].AllAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void FirstAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.FirstAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].FirstAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void FirstAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.FirstAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].FirstAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void FirstOrDefaultAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.FirstOrDefaultAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].FirstOrDefaultAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void FirstOrDefaultAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.FirstOrDefaultAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].FirstOrDefaultAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void LastAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.LastAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].LastAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void LastAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.LastAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].LastAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void LastOrDefaultAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.LastOrDefaultAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].LastOrDefaultAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void LastOrDefaultAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.LastOrDefaultAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].LastOrDefaultAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void SingleAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.SingleAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].SingleAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void SingleAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.SingleAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].SingleAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void SingleOrDefaultAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.SingleOrDefaultAsync(CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].SingleOrDefaultAsync(value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void SingleOrDefaultAsync_WithPredicate_Test()
        {
            var result = ExecuteAsync(contacts => contacts.SingleOrDefaultAsync(x => true, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].SingleOrDefaultAsync(x => True, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ElementAtAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.ElementAtAsync(3, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ElementAtAsync(3, value(System.Threading.CancellationToken))"));
        }

        [Test]
        public void ElementAtOrDefaultAsync_Test()
        {
            var result = ExecuteAsync(contacts => contacts.ElementAtOrDefaultAsync(3, CancellationToken.None));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Saleslogix.SData.Client.Test.Model.Contact[].ElementAtOrDefaultAsync(3, value(System.Threading.CancellationToken))"));
        }

        private static Expression ExecuteAsync<T>(Func<IQueryable<Contact>, Task<T>> func, T result = default(T))
        {
            var providerMock = new Mock<IQueryProvider>();
            Expression expr = null;
            var taskSource = new TaskCompletionSource<T>();
            taskSource.SetResult(result);
            providerMock.Setup(x => x.Execute<Task<T>>(It.IsAny<Expression>()))
                .Callback((Expression e) => expr = e)
                .Returns(taskSource.Task);
            var queryableMock = new Mock<IQueryable<Contact>>();
            queryableMock.Setup(x => x.Provider).Returns(providerMock.Object);
            queryableMock.Setup(x => x.Expression).Returns(Expression.Constant((new Contact[0]).AsQueryable()));
            func(queryableMock.Object).Wait(CancellationToken.None);
            return expr;
        }
#endif
    }
}