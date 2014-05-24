using System;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Framework
{
    [TestFixture]
    public class UriFormatterTests
    {
        /// <summary>
        /// The path segments property should be refreshed when a new uri
        /// is assigned to an existing uri formatter.
        /// </summary>
        [Test]
        public void Assign_New_Uri_To_Existing_Test()
        {
            var uri = new UriFormatter("http://test.com/sdata/-/-/-/");
            var expected = new[] {new UriPathSegment("sdata"), new UriPathSegment("-"), new UriPathSegment("-"), new UriPathSegment("-")};
            Assert.That(uri.PathSegments, Is.EquivalentTo(expected));

            uri.Uri = new Uri("http://localhost:3333/sdata/aw/dynamic/-/");
            expected = new[] {new UriPathSegment("sdata"), new UriPathSegment("aw"), new UriPathSegment("dynamic"), new UriPathSegment("-")};
            Assert.That(uri.PathSegments, Is.EquivalentTo(expected));
        }

        /// <summary>
        /// URI fragments should be supported to facilitate working with
        /// $schema redirection URIs.
        /// </summary>
        [Test]
        public void Support_Uri_Fragments_Test()
        {
            var uri = new UriFormatter("http://test.com/sdata/-/-/-/#one");
            Assert.That(uri.Fragment, Is.EqualTo("one"));

            uri.Host = "localhost";
            Assert.That(uri.Fragment, Is.EqualTo("one"));

            uri.Fragment = "two";
            Assert.That(uri.ToString(), Is.StringEnding("#two"));
        }

        [Test]
        public void Assign_Ampersand_In_Query_Arg_Test()
        {
            var uri = new SDataUri("http://localhost/sdata/aw/dynamic/-/accounts");
            uri["a"] = "&";
            uri["b"] = "&";

            Assert.That(uri["a"], Is.EqualTo("&"));
            Assert.That(uri["b"], Is.EqualTo("&"));
            Assert.That(uri.Query, Is.EqualTo("a=%26&b=%26"));
        }

        [Test]
        public void Assign_Ampersand_In_Query_Test()
        {
            var uri = new SDataUri("http://localhost/sdata/aw/dynamic/-/accounts") {Query = "a=%26&b=%26"};

            Assert.That(uri.Query, Is.EqualTo("a=%26&b=%26"));
            Assert.That(uri["a"], Is.EqualTo("&"));
            Assert.That(uri["b"], Is.EqualTo("&"));
        }

        /// <summary>
        /// Modifying the QueryArgs dictionary should cause the Query property
        /// and ToString method to update accordingly.
        /// </summary>
        [Test]
        public void Modifying_QueryArgs_Should_Update_Query_And_ToString_Test()
        {
            var uri = new UriFormatter("http://localhost");

            Assert.That(uri.Query, Is.Null.Or.Empty);
            Assert.That(uri.QueryArgs, Is.Empty);
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost/"));

            uri.QueryArgs.Add("orderBy", "name");

            Assert.That(uri.Query, Is.EqualTo("orderBy=name"));
            Assert.That(uri.QueryArgs.Count, Is.EqualTo(1));
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost/?orderBy=name"));

            uri.QueryArgs.Clear();

            Assert.That(uri.Query, Is.Null.Or.Empty);
            Assert.That(uri.QueryArgs, Is.Empty);
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost/"));
        }

        [Test]
        public void Clone_Test()
        {
            var before = new UriFormatter("http://localhost/sdata/aw/dynamic/-/accounts?select=Name");
            Assert.That(before.PathSegments, Is.Not.Null);
            var after = new UriFormatter(before);
            Assert.That(after.Scheme, Is.EqualTo(before.Scheme));
            Assert.That(after.Port, Is.EqualTo(before.Port));
            Assert.That(after.Host, Is.EqualTo(before.Host));
            Assert.That(after.Fragment, Is.EqualTo(before.Fragment));
            Assert.That(after.Path, Is.EqualTo(before.Path));
            Assert.That(after.PathSegments, Is.EqualTo(before.PathSegments));
            Assert.That(after.Query, Is.EqualTo(before.Query));
            Assert.That(after.QueryArgs, Is.EqualTo(before.QueryArgs));
            Assert.That(after.PathQuery, Is.EqualTo(before.PathQuery));
        }

        [Test]
        public void Modifying_QueryArgs_Should_Not_Require_Path_Parse_Test()
        {
            var uri = new Modifying_QueryArgs_Should_Not_Require_Path_Parse_Object("http://localhost:3333/sdata/aw/dynamic/-/accounts");
            uri["format"] = "html";
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost:3333/sdata/aw/dynamic/-/accounts?format=html"));
            Assert.That(uri.OnParsePathCalled, Is.False);
        }

        private class Modifying_QueryArgs_Should_Not_Require_Path_Parse_Object : UriFormatter
        {
            public bool OnParsePathCalled;

            public Modifying_QueryArgs_Should_Not_Require_Path_Parse_Object(string uri)
                : base(uri)
            {
            }

            protected override void OnParsePath()
            {
                OnParsePathCalled = true;
                base.OnParsePath();
            }
        }

        [Test]
        public void Modifying_PathSegments_Should_Not_Require_Query_Parse_Test()
        {
            var uri = new Modifying_PathSegments_Should_Not_Require_Query_Parse_object("http://localhost:3333/sdata/aw/dynamic/-/accounts");
            uri.AppendPath("nested");
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost:3333/sdata/aw/dynamic/-/accounts/nested"));
            Assert.That(uri.OnParseQueryCalled, Is.False);
        }

        private class Modifying_PathSegments_Should_Not_Require_Query_Parse_object : UriFormatter
        {
            public bool OnParseQueryCalled;

            public Modifying_PathSegments_Should_Not_Require_Query_Parse_object(string uri)
                : base(uri)
            {
            }

            protected override void OnParseQuery()
            {
                OnParseQueryCalled = true;
                base.OnParseQuery();
            }
        }

        [Test]
        public void Assign_Path_Null_Test()
        {
            var uri = new UriFormatter("http://localhost:3333/sdata/aw/dynamic/-/accounts?format=json") {Path = null};
            uri.AppendPath("hello", "world");
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost:3333/hello/world?format=json"));
        }

        [Test]
        public void Assign_Query_Null_Test()
        {
            var uri = new UriFormatter("http://localhost:3333/sdata/aw/dynamic/-/accounts?format=json") {Query = null};
            uri["hello"] = "world";
            Assert.That(uri.ToString(), Is.EqualTo("http://localhost:3333/sdata/aw/dynamic/-/accounts?hello=world"));
        }
    }
}