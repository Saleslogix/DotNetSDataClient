using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Saleslogix.SData.Client.Linq;
using Saleslogix.SData.Client.Test.Model;

namespace Saleslogix.SData.Client.Test.Linq
{
    [TestFixture]
    public class PropertyPathBuilderTests
    {
        [Test]
        public void Typed_Test()
        {
            AssertPropertyPath((Contact c) => c.LastName, "LastName");
        }

        [Test]
        public void Untyped_Test()
        {
            AssertPropertyPath((IDictionary<string, object> c) => c["LastName"], "LastName");
        }

        [Test]
        public void Nested_Typed_Test()
        {
            AssertPropertyPath((Contact c) => c.Address.City, "Address.City");
        }

        [Test]
        public void Nested_Untyped_Test()
        {
            AssertPropertyPath((IDictionary<string, object> c) => ((IDictionary<string, object>) c["Address"])["City"], "Address.City");
        }

        private static void AssertPropertyPath<TInput, TOutput>(Expression<Func<TInput, TOutput>> expr, string expected)
        {
            var actual = string.Join(".",
                PropertyPathBuilder.Build(expr.Body)
                    .Select(prop =>
                    {
                        var member = prop as MemberInfo;
                        return member != null ? member.Name : prop.ToString();
                    }).ToArray());
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}