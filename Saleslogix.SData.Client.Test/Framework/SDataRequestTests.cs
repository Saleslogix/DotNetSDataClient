using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Moq;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

#if !NETFX_CORE
using System.Reflection;
#endif

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Framework
{
    [TestFixture]
    public class SDataRequestTests
    {
        private Mock<IWebRequestCreate> _creatorMock;
        private Queue<HttpWebRequest> _requests;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _creatorMock = new Mock<IWebRequestCreate>();
            WebRequest.RegisterPrefix("test:", _creatorMock.Object);
            _requests = new Queue<HttpWebRequest>();
            _creatorMock.Setup(x => x.Create(It.IsAny<Uri>()))
                        .Returns((Uri uri) =>
                                     {
                                         var webRequest = _requests.Dequeue();
#if !NETFX_CORE
                                         typeof (HttpWebRequest).GetField("_Uri", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(webRequest, uri);
#endif
                                         return webRequest;
                                     });
        }

        [TearDown]
        public void TearDown()
        {
            Assert.That(_requests, Is.Empty);
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        [Test]
        public void Simple_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var proxy = new WebProxy("test://dummy");
            var cookies = new CookieContainer();
            var credentials = new NetworkCredential("admin", "");
            var request = new SDataRequest("test://localhost/sdata/invoices")
                               {
                                   ETag = "abc123",
                                   Timeout = 123,
                                   UserAgent = "test agent",
                                   Proxy = proxy,
                                   Accept = new[] {MediaType.Text, MediaType.Xml},
                                   Cookies = cookies,
                                   Credentials = credentials
                               };
            var response = request.GetResponse();

            Assert.That(webRequest.Timeout, Is.EqualTo(123));
            Assert.That(webRequest.UserAgent, Is.EqualTo("test agent"));
            Assert.That(webRequest.Proxy, Is.EqualTo(proxy));
            Assert.That(webRequest.Accept, Is.EqualTo("text/plain,application/xml"));
            Assert.That(webRequest.CookieContainer, Is.EqualTo(cookies));
            Assert.That(webRequest.Credentials, Is.EqualTo(credentials));
            Assert.That(webRequest.Headers[HttpRequestHeader.IfNoneMatch], Is.EqualTo("abc123"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(response.Content, Is.Null);
        }

        [Test]
        public void Redirect_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.Found);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection {{"Location", "test://localhost/sdata/invoices/redirect"}});
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            requestMock = new Mock<HttpWebRequest> {CallBase = true};
            responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices");
            var response = request.GetResponse();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(response.Location, Is.EqualTo("test://localhost/sdata/invoices/redirect"));
        }

        [Test]
        public void Text_Content_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            requestMock.Setup(x => x.GetRequestStream()).Returns(content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post, "hello");
            var response = request.GetResponse();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(content.ToArray(), Is.EqualTo(Encoding.UTF8.GetBytes("hello")));
        }

        [Test]
        public void Retry_Attmpts_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Throws(new WebException(null, WebExceptionStatus.Timeout));
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            requestMock = new Mock<HttpWebRequest> {CallBase = true};
            responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices");
            var response = request.GetResponse();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Multipart_FormData_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            requestMock.Setup(x => x.GetRequestStream()).Returns(content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Form = {{"id", "123"}}};
            var response = request.GetResponse();
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/form-data;"));
            Assert.That(text, Is.StringContaining("inline; name=id"));
            Assert.That(text, Is.StringContaining("123"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Multipart_File_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            requestMock.Setup(x => x.GetRequestStream()).Returns(content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Files = {new AttachedFile("text/plain", "hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}};
            var response = request.GetResponse();
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("attachment; filename=hello.txt"));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Multipart_Space_File_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            requestMock.Setup(x => x.GetRequestStream()).Returns(content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Files = {new AttachedFile("text/plain", "hel lo.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}};
            var response = request.GetResponse();
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("attachment; filename=\"hel lo.txt\""));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Multipart_Unicode_File_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            requestMock.Setup(x => x.GetRequestStream()).Returns(content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Files = {new AttachedFile("text/plain", "\x65b0\x65e5\x9244\x4f4f\x91d1.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}};
            var response = request.GetResponse();
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("attachment; filename*=utf-8''%E6%96%B0%E6%97%A5%E9%89%84%E4%BD%8F%E9%87%91.txt"));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Multipart_Mixed_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            requestMock.Setup(x => x.GetRequestStream()).Returns(content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post, "main content")
                         {
                             Form = {{"id", "123"}},
                             Files = {new AttachedFile("text/plain", "hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}
                         };
            var response = request.GetResponse();
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("main content"));
            Assert.That(text, Is.StringContaining("inline; name=id"));
            Assert.That(text, Is.StringContaining("123"));
            Assert.That(text, Is.StringContaining("attachment; filename=hello.txt"));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void UseHttpMethodOverride_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            requestMock.Setup(x => x.GetResponse()).Returns(responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Put)
                {
                    UseHttpMethodOverride = true
                };
            request.GetResponse();

            Assert.That(webRequest.Headers["X-HTTP-Method-Override"], Is.EqualTo("PUT"));
        }
#endif

        [Test]
        public void Async_Simple_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

#if !PCL && !SILVERLIGHT
            var proxy = new WebProxy("test://dummy");
#endif
            var cookies = new CookieContainer();
            var credentials = new NetworkCredential("admin", "");
            var request = new SDataRequest("test://localhost/sdata/invoices")
                              {
                                  ETag = "abc123",
#if !PCL && !NETFX_CORE && !SILVERLIGHT
                                  Timeout = 123,
                                  UserAgent = "test agent",
#endif
#if !PCL && !SILVERLIGHT
                                  Proxy = proxy,
#endif
                                  Accept = new[] {MediaType.Text, MediaType.Xml},
                                  Cookies = cookies,
                                  Credentials = credentials
                              };
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));

#if !PCL && !NETFX_CORE && !SILVERLIGHT
            Assert.That(webRequest.Timeout, Is.EqualTo(123));
            Assert.That(webRequest.UserAgent, Is.EqualTo("test agent"));
#endif
#if !PCL && !SILVERLIGHT
            Assert.That(webRequest.Proxy, Is.EqualTo(proxy));
#endif
            Assert.That(webRequest.Accept, Is.EqualTo("text/plain,application/xml"));
            Assert.That(webRequest.CookieContainer, Is.EqualTo(cookies));
            Assert.That(webRequest.Credentials, Is.EqualTo(credentials));
            Assert.That(webRequest.Headers[HttpRequestHeader.IfNoneMatch], Is.EqualTo("abc123"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(response.Content, Is.Null);
        }

        [Test]
        public void Async_Redirect_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.Found);
            var headers = new WebHeaderCollection();
            headers["Location"] = "test://localhost/sdata/invoices/redirect";
            responseMock.Setup(x => x.Headers).Returns(headers);
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            requestMock = new Mock<HttpWebRequest> {CallBase = true};
            responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            SetupGetResponseAsync(requestMock, responseMock.Object);
            webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices");
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(response.Location, Is.EqualTo("test://localhost/sdata/invoices/redirect"));
        }

        [Test]
        public void Async_Text_Content_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            SetupGetRequestStreamAsync(requestMock, content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post, "hello");
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(content.ToArray(), Is.EqualTo(Encoding.UTF8.GetBytes("hello")));
        }

        [Test]
        public void Async_Retry_Attmpts_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            const WebExceptionStatus status =
#if PCL || NETFX_CORE || SILVERLIGHT
                WebExceptionStatus.ConnectFailure;
#else
                WebExceptionStatus.Timeout;
#endif
            SetupGetResponseAsync(requestMock, new WebException(null, status));
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            requestMock = new Mock<HttpWebRequest> {CallBase = true};
            responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices");
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Async_Multipart_FormData_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            SetupGetRequestStreamAsync(requestMock, content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Form = {{"id", "123"}}};
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/form-data;"));
            Assert.That(text, Is.StringContaining("inline; name=id"));
            Assert.That(text, Is.StringContaining("123"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Async_Multipart_File_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            SetupGetRequestStreamAsync(requestMock, content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Files = {new AttachedFile("text/plain", "hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}};
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("attachment; filename=hello.txt"));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Async_Multipart_Unicode_File_Only_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            SetupGetRequestStreamAsync(requestMock, content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post) {Files = {new AttachedFile("text/plain", "\x65b0\x65e5\x9244\x4f4f\x91d1.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}};
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("attachment; filename*=utf-8''%E6%96%B0%E6%97%A5%E9%89%84%E4%BD%8F%E9%87%91.txt"));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Async_Multipart_Mixed_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            var content = new MemoryStream();
            SetupGetRequestStreamAsync(requestMock, content);
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Post, "main content")
                         {
                             Form = {{"id", "123"}},
                             Files = {new AttachedFile("text/plain", "hello.txt", new MemoryStream(Encoding.UTF8.GetBytes("world")))}
                         };
            var response = request.EndGetResponse(request.BeginGetResponse(null, null));
            var text = Encoding.UTF8.GetString(content.ToArray());

            Assert.That(webRequest.ContentType, Is.StringStarting("multipart/related;"));
            Assert.That(text, Is.StringContaining("main content"));
            Assert.That(text, Is.StringContaining("inline; name=id"));
            Assert.That(text, Is.StringContaining("123"));
            Assert.That(text, Is.StringContaining("attachment; filename=hello.txt"));
            Assert.That(text, Is.StringContaining("world"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Async_UseHttpMethodOverride_Test()
        {
            var requestMock = new Mock<HttpWebRequest> {CallBase = true};
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            SetupGetResponseAsync(requestMock, responseMock.Object);
            var webRequest = requestMock.Object;
            webRequest.Headers = new WebHeaderCollection();
            _requests.Enqueue(webRequest);

            var request = new SDataRequest("test://localhost/sdata/invoices", HttpMethod.Put)
                {
                    UseHttpMethodOverride = true
                };
            request.EndGetResponse(request.BeginGetResponse(null, null));

            Assert.That(webRequest.Headers["X-HTTP-Method-Override"], Is.EqualTo("PUT"));
        }

        private static void SetupGetRequestStreamAsync(Mock<HttpWebRequest> requestMock, Stream content)
        {
            var asyncMock = new Mock<IAsyncResult>();
            requestMock.Setup(x => x.BeginGetRequestStream(It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Callback((AsyncCallback callback, object state) => callback(asyncMock.Object));
            requestMock.Setup(x => x.EndGetRequestStream(It.IsAny<IAsyncResult>())).Returns(content);
        }

        private static void SetupGetResponseAsync(Mock<HttpWebRequest> requestMock, WebResponse response)
        {
            var asyncMock = new Mock<IAsyncResult>();
            requestMock.Setup(x => x.BeginGetResponse(It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Callback((AsyncCallback callback, object state) => callback(asyncMock.Object));
            requestMock.Setup(x => x.EndGetResponse(It.IsAny<IAsyncResult>())).Returns(response);
        }

        private static void SetupGetResponseAsync(Mock<HttpWebRequest> requestMock, Exception ex)
        {
            var asyncMock = new Mock<IAsyncResult>();
            requestMock.Setup(x => x.BeginGetResponse(It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Callback((AsyncCallback callback, object state) => callback(asyncMock.Object));
            requestMock.Setup(x => x.EndGetResponse(It.IsAny<IAsyncResult>())).Throws(ex);
        }
    }
}