using System;
using System.Collections.Generic;
using System.Net;
using Moq;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataClientTests
    {
        [Test]
        public void Execute_Test()
        {
            var requestMock = new Mock<SDataRequest>(null, null, null);
            requestMock.Setup(x => x.GetResponse()).Returns(new SDataResponse(HttpStatusCode.OK, null, null, null, null, null, null, null, null));
            SDataUri requestUri = null;
            var requestFactory = new Func<string, SDataRequest>(uri =>
            {
                requestUri = new SDataUri(uri);
                return requestMock.Object;
            });
            var client = new SDataClient("test://dummy", requestFactory);
            var content = new object();
            var file = new AttachedFile(null, null, null);
            var parms = new SDataParameters
                {
                    StartIndex = 1,
                    Count = 2,
                    Where = "where",
                    OrderBy = "order_by",
                    Search = "search",
                    Include = "include",
                    Select = "select",
                    Precedence = 3,
                    IncludeSchema = true,
                    ReturnDelta = true,
                    TrackingId = "tracking_id",
                    Format = MediaType.ImagePng,
                    Language = "language",
                    Version = "version",
                    Path = "path",
                    ExtensionArgs = {{"foo", "bar"}},
                    Method = HttpMethod.Put,
                    Content = content,
                    ContentType = MediaType.ImageTiff,
                    ETag = "etag",
                    Form = {{"hello", "world"}},
                    Files = {file},
                    Accept = new[] {MediaType.ImageJpeg}
                };
            client.Execute(parms);

            Assert.That(requestUri, Is.Not.Null);
            Assert.That(requestUri.StartIndex, Is.EqualTo(1));
            Assert.That(requestUri.Count, Is.EqualTo(2));
            Assert.That(requestUri.Where, Is.EqualTo("where"));
            Assert.That(requestUri.OrderBy, Is.EqualTo("order_by"));
            Assert.That(requestUri.Search, Is.EqualTo("search"));
            Assert.That(requestUri.Include, Is.EqualTo("include"));
            Assert.That(requestUri.Select, Is.EqualTo("select"));
            Assert.That(requestUri.Precedence, Is.EqualTo(3));
            Assert.That(requestUri.IncludeSchema, Is.EqualTo(true));
            Assert.That(requestUri.ReturnDelta, Is.EqualTo(true));
            Assert.That(requestUri.TrackingId, Is.EqualTo("tracking_id"));
            Assert.That(requestUri.Format, Is.EqualTo(MediaType.ImagePng));
            Assert.That(requestUri.Language, Is.EqualTo("language"));
            Assert.That(requestUri.Version, Is.EqualTo("version"));
            Assert.That(requestUri.Path, Is.EqualTo("path"));
            Assert.That(requestUri["_foo"], Is.EqualTo("bar"));
            Assert.That(requestMock.Object.Method, Is.EqualTo(HttpMethod.Put));
            Assert.That(requestMock.Object.Content, Is.EqualTo(content));
            Assert.That(requestMock.Object.ContentType, Is.EqualTo(MediaType.ImageTiff));
            Assert.That(requestMock.Object.ETag, Is.EqualTo("etag"));
            Assert.That(requestMock.Object.Form["hello"], Is.EqualTo("world"));
            Assert.That(requestMock.Object.Files[0], Is.EqualTo(file));
            Assert.That(requestMock.Object.Accept, Is.EqualTo(new[] {MediaType.ImageJpeg}));
            Assert.That(requestMock.Object.AcceptLanguage, Is.EqualTo("language"));
        }

        [Test]
        public void Execute_Batch_Test()
        {
            var requestMock = new Mock<SDataRequest>(null, null, null);
            requestMock.Setup(x => x.GetResponse()).Returns(new SDataResponse(HttpStatusCode.OK, null, null, null, null, null, null, null, null));
            SDataUri requestUri = null;
            var requestFactory = new Func<string, SDataRequest>(uri =>
            {
                requestUri = new SDataUri(uri);
                return requestMock.Object;
            });
            var client = new SDataClient("test://dummy", requestFactory);
            var file1 = new AttachedFile(null, null, null);
            var params1 = new SDataParameters
                {
                    Include = "include1",
                    Select = "select1",
                    Precedence = 1,
                    Format = MediaType.ImagePng,
                    Language = "language",
                    Version = "version",
                    Path = "path",
                    ExtensionArgs = {{"foo1", "bar1"}},
                    Method = HttpMethod.Post,
                    Content = new object(),
                    ContentType = MediaType.ImageTiff,
                    ETag = "etag1",
                    Form = {{"hello1", "world1"}},
                    Files = {file1},
                    Accept = new[] {MediaType.ImageJpeg}
                };
            var file2 = new AttachedFile(null, null, null);
            var resource2 = new SDataResource {Key = "key2"};
            resource2["foo"] = "bar";
            var params2 = new SDataParameters
                {
                    Include = "include2",
                    Select = "select2",
                    Precedence = 2,
                    Format = MediaType.ImagePng,
                    Language = "language",
                    Version = "version",
                    Path = "path",
                    ExtensionArgs = {{"foo2", "bar2"}},
                    Method = HttpMethod.Put,
                    Content = resource2,
                    ContentType = MediaType.ImageTiff,
                    ETag = "etag2",
                    Form = {{"hello2", "world2"}},
                    Files = {file2},
                    Accept = new[] {MediaType.Css}
                };
            client.ExecuteBatch<SDataResource>(new[] {params1, params2});

            var resources = requestMock.Object.Content as IList<SDataResource>;
            Assert.That(resources, Is.Not.Null);
            Assert.That(requestUri, Is.Not.Null);
            Assert.That(requestUri.Include, Is.EqualTo("include1,include2"));
            Assert.That(requestUri.Select, Is.EqualTo("select1,select2"));
            Assert.That(requestUri.Precedence, Is.EqualTo(2));
            Assert.That(requestUri.Format, Is.EqualTo(MediaType.ImagePng));
            Assert.That(requestUri.Language, Is.EqualTo("language"));
            Assert.That(requestUri.Version, Is.EqualTo("version"));
            Assert.That(requestUri.Path, Is.EqualTo("path/$batch"));
            Assert.That(requestUri["_foo1"], Is.EqualTo("bar1"));
            Assert.That(requestUri["_foo2"], Is.EqualTo("bar2"));
            Assert.That(requestMock.Object.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(resources[0].HttpMethod, Is.EqualTo(HttpMethod.Post));
            Assert.That(resources[1].HttpMethod, Is.EqualTo(HttpMethod.Put));
            Assert.That(resources[0].Url, Is.Null);
            Assert.That(resources[1].Url, Is.EqualTo(new Uri("test://dummy/path('key2')?include=include2&select=select2&precedence=2")));
            Assert.That(requestMock.Object.ContentType, Is.EqualTo(MediaType.ImageTiff));
            Assert.That(resources[0].IfMatch, Is.EqualTo("etag1"));
            Assert.That(resources[1].IfMatch, Is.EqualTo("etag2"));
            Assert.That(requestMock.Object.Form["hello1"], Is.EqualTo("world1"));
            Assert.That(requestMock.Object.Form["hello2"], Is.EqualTo("world2"));
            Assert.That(requestMock.Object.Files[0], Is.EqualTo(file1));
            Assert.That(requestMock.Object.Files[1], Is.EqualTo(file2));
            Assert.That(requestMock.Object.Accept, Is.EqualTo(new[] {MediaType.ImageJpeg, MediaType.Css}));
            Assert.That(requestMock.Object.AcceptLanguage, Is.EqualTo("language"));
        }
    }
}