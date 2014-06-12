using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Moq;
using NUnit.Framework;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Framework
{
    [TestFixture]
    public class SDataResponseTests
    {
        [Test]
        public void No_Content_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            responseMock.Setup(x => x.ContentType).Returns("text/plain");
            var headers = new WebHeaderCollection();
            headers["ETag"] = "abc123";
            responseMock.Setup(x => x.Headers).Returns(headers);
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("hello")));

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Text_Content_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            responseMock.Setup(x => x.ContentType).Returns("text/plain");
            var headers = new WebHeaderCollection();
            headers["ETag"] = "abc123";
            responseMock.Setup(x => x.Headers).Returns(headers);
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("hello")));

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ContentType, Is.EqualTo(MediaType.Text));
            Assert.That(response.Content, Is.EqualTo("hello"));
            Assert.That(response.ETag, Is.EqualTo("abc123"));
        }

        [Test]
        public void Unknown_Content_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            responseMock.Setup(x => x.ContentType).Returns("application/custom");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            var content = Encoding.UTF8.GetBytes("hello");
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream(content));

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ContentType, Is.Null);
            Assert.That(response.Content, Is.EqualTo(content));
        }

        [Test]
        public void Invalid_ContentType_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            responseMock.Setup(x => x.ContentType).Returns("dummy");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ContentType, Is.Null);
        }

        [Test]
        public void Multipart_FormData_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            responseMock.Setup(x => x.ContentType).Returns("multipart/form-data; boundary=8d04bbc8aee5755");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            const string data = @"--8d04bbc8aee5755
Content-Type: text/plain

--8d04bbc8aee5755
Content-Disposition: inline; name=id

123
--8d04bbc8aee5755--";
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Form, Has.Count.EqualTo(1));
            Assert.That(response.Form["id"], Is.EqualTo("123"));
        }

        [Test]
        public void Multipart_File_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            responseMock.Setup(x => x.ContentType).Returns("multipart/related; boundary=8d04bbc8aee5755");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            const string data = @"--8d04bbc8aee5755
Content-Type: text/plain

--8d04bbc8aee5755
Content-Disposition: attachment; filename=hello.txt

world
--8d04bbc8aee5755--";
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Files, Has.Count.EqualTo(1));
            Assert.That(response.Files[0].FileName, Is.EqualTo("hello.txt"));
            Assert.That(new StreamReader(response.Files[0].Stream).ReadToEnd(), Is.EqualTo("world"));
        }

        [Test]
        public void Tracking_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.Accepted);
            responseMock.Setup(x => x.ContentType).Returns("image/png");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            var handlerMock = new Mock<IContentHandler>();
            handlerMock.Setup(x => x.ReadFrom(It.IsAny<Stream>()))
                       .Returns(new Dictionary<string, object>
                                    {
                                        {"phase", "_Phase"},
                                        {"phaseDetail", "_PhaseDetail"},
                                        {"progress", 1.2},
                                        {"elapsedSeconds", 2.3},
                                        {"remainingSeconds", 3.4},
                                        {"pollingMillis", 5L}
                                    });
            ContentManager.SetHandler(MediaType.ImagePng, handlerMock.Object);

            var response = new SDataResponse(responseMock.Object, null);

            ContentManager.SetHandler(MediaType.ImagePng, null);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
            Assert.That(response.Content, Is.TypeOf<Tracking>());
            var tracking = (Tracking) response.Content;
            Assert.That(tracking.Phase, Is.EqualTo("_Phase"));
            Assert.That(tracking.PhaseDetail, Is.EqualTo("_PhaseDetail"));
            Assert.That(tracking.Progress, Is.EqualTo(1.2));
            Assert.That(tracking.ElapsedSeconds, Is.EqualTo(2.3));
            Assert.That(tracking.RemainingSeconds, Is.EqualTo(3.4));
            Assert.That(tracking.PollingMillis, Is.EqualTo(5L));
        }

        [Test]
        public void Diagnoses_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            responseMock.Setup(x => x.ContentType).Returns("image/png");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            var handlerMock = new Mock<IContentHandler>();
            handlerMock.Setup(x => x.ReadFrom(It.IsAny<Stream>()))
                       .Returns(new[]
                                    {
                                        new Dictionary<string, object>
                                            {
                                                {"severity", "Error"},
                                                {"sdataCode", "BadUrlSyntax"},
                                                {"applicationCode", "_ApplicationCode"},
                                                {"message", "_Message"},
                                                {"stackTrace", "_StackTrace"},
                                                {"payloadPath", "_PayloadPath"}
                                            }
                                    });
            ContentManager.SetHandler(MediaType.ImagePng, handlerMock.Object);

            SDataException sDataEx = null;
            try
            {
                new SDataResponse(responseMock.Object, null);
            }
            catch (SDataException ex)
            {
                sDataEx = ex;
            }

            ContentManager.SetHandler(MediaType.ImagePng, null);

            Assert.That(sDataEx, Is.Not.Null);
            Assert.That(sDataEx.Diagnoses, Has.Count.EqualTo(1));
            var diagnosis = sDataEx.Diagnoses[0];
            Assert.That(diagnosis.Severity, Is.EqualTo(Severity.Error));
            Assert.That(diagnosis.SDataCode, Is.EqualTo(DiagnosisCode.BadUrlSyntax));
            Assert.That(diagnosis.ApplicationCode, Is.EqualTo("_ApplicationCode"));
            Assert.That(diagnosis.Message, Is.EqualTo("_Message"));
            Assert.That(diagnosis.StackTrace, Is.EqualTo("_StackTrace"));
            Assert.That(diagnosis.PayloadPath, Is.EqualTo("_PayloadPath"));
        }

        [Test]
        public void Diagnosis_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            responseMock.Setup(x => x.ContentType).Returns("image/png");
            responseMock.Setup(x => x.Headers).Returns(new WebHeaderCollection());
            responseMock.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            var handlerMock = new Mock<IContentHandler>();
            handlerMock.Setup(x => x.ReadFrom(It.IsAny<Stream>()))
                       .Returns(new Dictionary<string, object>
                                    {
                                        {"severity", "Error"},
                                        {"sdataCode", "BadUrlSyntax"},
                                        {"applicationCode", "_ApplicationCode"},
                                        {"message", "_Message"},
                                        {"stackTrace", "_StackTrace"},
                                        {"payloadPath", "_PayloadPath"}
                                    });
            ContentManager.SetHandler(MediaType.ImagePng, handlerMock.Object);

            SDataException sDataEx = null;
            try
            {
                new SDataResponse(responseMock.Object, null);
            }
            catch (SDataException ex)
            {
                sDataEx = ex;
            }

            ContentManager.SetHandler(MediaType.ImagePng, null);

            Assert.That(sDataEx, Is.Not.Null);
            Assert.That(sDataEx.Diagnoses, Has.Count.EqualTo(1));
            var diagnosis = sDataEx.Diagnoses[0];
            Assert.That(diagnosis.Severity, Is.EqualTo(Severity.Error));
            Assert.That(diagnosis.SDataCode, Is.EqualTo(DiagnosisCode.BadUrlSyntax));
            Assert.That(diagnosis.ApplicationCode, Is.EqualTo("_ApplicationCode"));
            Assert.That(diagnosis.Message, Is.EqualTo("_Message"));
            Assert.That(diagnosis.StackTrace, Is.EqualTo("_StackTrace"));
            Assert.That(diagnosis.PayloadPath, Is.EqualTo("_PayloadPath"));
        }

        [Test]
        public void Expires_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            var headers = new WebHeaderCollection();
            headers["Expires"] = "Mon, 01 Dec 2014 16:17:18 GMT";
            responseMock.Setup(x => x.Headers).Returns(headers);

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.Expires, Is.EqualTo(new DateTimeOffset(2014, 12, 1, 16, 17, 18, TimeSpan.Zero)));
        }

        [Test]
        public void RetryAfter_Date_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            var headers = new WebHeaderCollection();
            headers["Retry-After"] = "Mon, 01 Dec 2014 16:17:18 GMT";
            responseMock.Setup(x => x.Headers).Returns(headers);

            var response = new SDataResponse(responseMock.Object, null);

            Assert.That(response.RetryAfter, Is.EqualTo(new DateTimeOffset(2014, 12, 1, 16, 17, 18, TimeSpan.Zero)));
        }

        [Test]
        public void RetryAfter_Number_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.NoContent);
            var headers = new WebHeaderCollection();
            headers["Retry-After"] = "120";
            responseMock.Setup(x => x.Headers).Returns(headers);

            var before = DateTimeOffset.UtcNow;
            var response = new SDataResponse(responseMock.Object, null);
            var after = DateTimeOffset.UtcNow;

            Assert.That(response.RetryAfter, Is.GreaterThanOrEqualTo(before.AddSeconds(120)));
            Assert.That(response.RetryAfter, Is.LessThanOrEqualTo(after.AddSeconds(120)));
        }
    }
}