using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Parsing.Structure;
using Saleslogix.SData.Client.Linq;
using Saleslogix.SData.Client.Test.Model;

namespace Saleslogix.SData.Client.Test.Linq
{
    [TestFixture]
    public class PropertyPathExtractionVisitorTests
    {
        [Test]
        public void Empty_Test()
        {
            AssertSelectPath((Contact c) => c, "*");
            AssertSelectPath((IDictionary<string, object> c) => c, "*");
        }

        [Test]
        public void Simple_Test()
        {
            AssertSelectPath((Contact c) => c.LastName, "LastName");
            AssertSelectPath((IDictionary<string, object> c) => (string) c["LastName"], "LastName");
        }

        [Test]
        public void Object_Test()
        {
            AssertSelectPath((Contact c) => c.Address, "Address/*");
            AssertSelectPath((IDictionary<string, object> c) => (IDictionary<string, object>) c["Address"], "Address/*");
        }

        [Test]
        public void Collection_Test()
        {
            AssertSelectPath((Contact c) => c.Addresses, "Addresses/*");
            AssertSelectPath((IDictionary<string, object> c) => (ICollection<object>) c["Addresses"], "Addresses/*");
        }

        [Test]
        public void Object_Simple_Test()
        {
            AssertSelectPath((SalesOrder s) => s.Contact.LastName, "Contact/LastName");
            AssertSelectPath((IDictionary<string, object> s) => (string) ((IDictionary<string, object>) s["Contact"])["LastName"], "Contact/LastName");
        }

        [Test]
        public void Object_Object_Test()
        {
            AssertSelectPath((SalesOrder s) => s.Contact.Address, "Contact/Address/*");
            AssertSelectPath((IDictionary<string, object> s) => (IDictionary<string, object>) ((IDictionary<string, object>) s["Contact"])["Address"], "Contact/Address/*");
        }

        [Test]
        public void Object_Collection_Test()
        {
            AssertSelectPath((SalesOrder s) => s.Contact.Addresses, "Contact/Addresses/*");
            AssertSelectPath((IDictionary<string, object> s) => (ICollection<object>) ((IDictionary<string, object>) s["Contact"])["Addresses"], "Contact/Addresses/*");
        }

        [Test]
        public void Collection_Empty_Test()
        {
            AssertSelectPath((Organization o) => o.Contacts.Select(c => c), "Contacts/*");
            AssertSelectPath((IDictionary<string, object> o) => ((ICollection<IDictionary<string, object>>) o["Contacts"]).Select(c => c), "Contacts/*");
        }

        [Test]
        public void Collection_Simple_Test()
        {
            AssertSelectPath((Organization o) => o.Contacts.Select(c => c.LastName), "Contacts/LastName");
            AssertSelectPath((IDictionary<string, object> o) => ((ICollection<IDictionary<string, object>>) o["Contacts"]).Select(c => (string) c["LastName"]), "Contacts/LastName");
        }

        [Test]
        public void Collection_Object_Test()
        {
            //AssertSelectPath((Organization o) => o.Contacts.Select(c => c.Address), "Contacts/Address/*");
            AssertSelectPath((IDictionary<string, object> o) => ((ICollection<IDictionary<string, object>>) o["Contacts"]).Select(c => (IDictionary<string, object>) c["Address"]), "Contacts/Address/*");
        }

        [Test]
        public void Collection_Collection_Test()
        {
            AssertSelectPath((Organization o) => o.Contacts.Select(c => c.Addresses), "Contacts/Addresses/*");
            AssertSelectPath((IDictionary<string, object> o) => ((ICollection<IDictionary<string, object>>) o["Contacts"]).Select(c => (ICollection<object>) c["Addresses"]), "Contacts/Addresses/*");
        }

        [Test]
        public void Multiple_Simple_Test()
        {
            AssertSelectPath((Contact c) => new {c.FirstName, c.LastName}, "FirstName", "LastName");
            AssertSelectPath((IDictionary<string, object> c) => new
                {
                    FirstName = (string) c["FirstName"],
                    LastName = (string) c["LastName"]
                }, "FirstName", "LastName");
        }

        [Test]
        public void Multiple_Object_Test()
        {
            AssertSelectPath((Contact c) => new {c.Address.Street, c.Address.City}, "Address/Street", "Address/City");
            AssertSelectPath((IDictionary<string, object> c) => new
                {
                    Street = (string) ((IDictionary<string, object>) c["Address"])["Street"],
                    City = (string) ((IDictionary<string, object>) c["Address"])["City"]
                }, "Address/Street", "Address/City");
        }

        [Test]
        public void Multiple_Collection_Test()
        {
            AssertSelectPath((Contact c) => new {Addresses = c.Addresses.Select(a => new {a.Street, a.City})}, "Addresses/Street", "Addresses/City");
            AssertSelectPath((IDictionary<string, object> c) => new
                {
                    Addresses = ((ICollection<IDictionary<string, object>>) c["Addresses"]).Select(a => new
                        {
                            Street = (string) a["Street"],
                            City = (string) a["City"]
                        })
                }, "Addresses/Street", "Addresses/City");
        }

        [Test]
        public void Simplify_SimpleTest()
        {
            AssertSelectPath((Contact c) => new {c, c.LastName}, "*");
            AssertSelectPath((IDictionary<string, object> c) => new
                {
                    c,
                    LastName = (string) c["LastName"]
                }, "*");
        }

        [Test]
        public void Simplify_Object_Test()
        {
            AssertSelectPath((Contact c) => new {c.Address, c.Address.City}, "Address/*");
            AssertSelectPath((IDictionary<string, object> c) => new
                {
                    Address = (IDictionary<string, object>) c["Address"],
                    LastName = (string) ((IDictionary<string, object>) c["Address"])["LastName"]
                }, "Address/*");
        }

        [Test]
        public void Simplify_Collection_Test()
        {
            AssertSelectPath((Contact c) => new {c.Addresses, Cities = c.Addresses.Select(a => a.City)}, "Addresses/*");
            AssertSelectPath((IDictionary<string, object> c) => new
                {
                    Addresses = (ICollection<object>) c["Addresses"],
                    Cities = ((ICollection<IDictionary<string, object>>) c["Addresses"]).Select(a => (string) a["LastName"])
                }, "Addresses/*");
        }

        [Test]
        public void Simplify_Mixed_Test()
        {
            AssertSelectPath((Organization o) => new {o.Contacts, Cities = o.Contacts.Select(c => c.Address.City)}, "Contacts/*", "Contacts/Address/City");
            AssertSelectPath((IDictionary<string, object> o) => new
                {
                    Contacts = (ICollection<object>) o["Contacts"],
                    Cities = ((ICollection<IDictionary<string, object>>) o["Contacts"]).Select(c => (string) ((IDictionary<string, object>) c["Address"])["City"])
                }, "Contacts/*", "Contacts/Address/City");
        }

        [Test]
        public void ProtocolProperty_Simple_Test()
        {
            AssertSelectPath((Contact c) => c.Key);
            AssertSelectPath((IDictionary<string, object> c) => (string) c["$key"]);
        }

        [Test]
        public void ProtocolProperty_Object_Test()
        {
            AssertSelectPath((Contact c) => c.Address.Key, "Address");
            AssertSelectPath((IDictionary<string, object> c) => (string) ((IDictionary<string, object>) c["Address"])["$key"], "Address");
        }

        [Test]
        public void ProtocolProperty_Collection_Test()
        {
            AssertSelectPath((Contact c) => c.Addresses.Select(a => a.Key), "Addresses");
            AssertSelectPath((IDictionary<string, object> c) => ((ICollection<IDictionary<string, object>>) c["Addresses"]).Select(a => (string) a["$key"]), "Addresses");
        }

        [Test]
        public void Wildcards_Count_Test()
        {
            AssertSelectPath((Contact c) => c.Addresses.Count(), "Addresses");
            //TODO: untyped
        }

        [Test]
        public void Wildcards_First_Test()
        {
            AssertSelectPath((Contact c) => c.Addresses.First(), "Addresses/*");
            //TODO: untyped
        }

        [Test]
        public void Fetch_Invalid_Method_Expression_Test()
        {
            Assert.That(() => AssertFetchPath((Contact c) => c.ToString()), Throws.Exception.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Fetch_Invalid_Unary_Expression_Test()
        {
            Assert.That(() => AssertFetchPath((Contact c) => !c.Active), Throws.Exception.TypeOf<NotSupportedException>());
        }

        private static void AssertSelectPath<TInput, TOutput>(Expression<Func<TInput, TOutput>> expr, params string[] expected)
        {
            var actual = PropertyPathExtractionVisitor.ExtractPaths(expr.Body, ExpressionTreeParser.CreateDefaultNodeTypeProvider(), true, false, NamingScheme.Basic, "/");
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static void AssertFetchPath<TInput, TOutput>(Expression<Func<TInput, TOutput>> expr, params string[] expected)
        {
            var actual = PropertyPathExtractionVisitor.ExtractPaths(expr.Body, ExpressionTreeParser.CreateDefaultNodeTypeProvider(), false, false, NamingScheme.Basic, "/");
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}