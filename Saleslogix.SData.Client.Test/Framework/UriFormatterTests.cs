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
            var expected = new[] {new UriPathSegment("-"), new UriPathSegment("-"), new UriPathSegment("-")};
            Assert.That(uri.PathSegments, Is.EquivalentTo(expected));

            uri.Uri = new Uri("http://localhost:3333/sdata/aw/dynamic/-/");
            expected = new[] {new UriPathSegment("aw"), new UriPathSegment("dynamic"), new UriPathSegment("-")};
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
            var uri = new SDataUri("http://localhost:2001/sdata/aw/dynamic/-/accounts");
            uri["a"] = "&";
            uri["b"] = "&";

            Assert.That(uri["a"], Is.EqualTo("&"));
            Assert.That(uri["b"], Is.EqualTo("&"));
            Assert.That(uri.Query, Is.EqualTo("a=%26&b=%26"));
        }

        [Test]
        public void Assign_Ampersand_In_Query_Test()
        {
            var uri = new SDataUri("http://localhost:2001/sdata/aw/dynamic/-/accounts") {Query = "a=%26&b=%26"};

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

            Assert.That(string.IsNullOrEmpty(uri.Query));
            Assert.That(uri.QueryArgs.Count == 0);
            Assert.That(uri.ToString() == "http://localhost/");

            uri.QueryArgs.Add("orderBy", "name");

            Assert.That(uri.Query == "orderBy=name");
            Assert.That(uri.QueryArgs.Count == 1);
            Assert.That(uri.ToString() == "http://localhost/?orderBy=name");

            uri.QueryArgs.Clear();

            Assert.That(string.IsNullOrEmpty(uri.Query));
            Assert.That(uri.QueryArgs.Count == 0);
            Assert.That(uri.ToString() == "http://localhost/");
        }

        [Test]
        public void Clone_Test()
        {
            var before = new UriFormatter("http://localhost/sdata/aw/dynamic/-/accounts?select=Name");
            var dummy = before.PathSegments;
            var after = new UriFormatter(before);
            Assert.That(after.Scheme, Is.EqualTo(before.Scheme));
            Assert.That(after.Port, Is.EqualTo(before.Port));
            Assert.That(after.Host, Is.EqualTo(before.Host));
            Assert.That(after.PathPrefix, Is.EqualTo(before.PathPrefix));
            Assert.That(after.Server, Is.EqualTo(before.Server));
            Assert.That(after.Fragment, Is.EqualTo(before.Fragment));
            Assert.That(after.Path, Is.EqualTo(before.Path));
            Assert.That(after.PathSegments, Is.EqualTo(before.PathSegments));
            Assert.That(after.Query, Is.EqualTo(before.Query));
            Assert.That(after.QueryArgs, Is.EqualTo(before.QueryArgs));
            Assert.That(after.PathQuery, Is.EqualTo(before.PathQuery));
        }
    }
}