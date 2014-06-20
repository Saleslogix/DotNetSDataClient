using System.IO;
using System.Net;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

#if !NETFX_CORE && !SILVERLIGHT
using System.Runtime.Serialization.Formatters.Binary;
#endif

#if !NET_2_0 && !NET_3_5
using System.Collections.Generic;
using Moq;
using Saleslogix.SData.Client.Content;
#endif

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Framework
{
    [TestFixture]
    public class SDataExceptionTests
    {
#if !NET_2_0 && !NET_3_5
        [Test]
        public void Diagnoses_Test()
        {
            var responseMock = new Mock<HttpWebResponse>();
            responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            responseMock.Setup(x => x.ContentType).Returns("image/png");
            responseMock.Setup(x => x.GetResponseStream()).Returns(default(Stream));
            var webEx = new WebException("error", null, WebExceptionStatus.ConnectFailure, responseMock.Object);

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

            var sDataEx = new SDataException(webEx);

            ContentManager.SetHandler(MediaType.ImagePng, null);

            Assert.That(sDataEx.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
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
            responseMock.Setup(x => x.GetResponseStream()).Returns(default(Stream));
            var webEx = new WebException("error", null, WebExceptionStatus.ConnectFailure, responseMock.Object);

            var handlerMock = new Mock<IContentHandler>();
            handlerMock.Setup(x => x.ReadFrom(It.IsAny<Stream>()))
                       .Returns(
                           new Dictionary<string, object>
                               {
                                   {"severity", "Error"},
                                   {"sdataCode", "BadUrlSyntax"},
                                   {"applicationCode", "_ApplicationCode"},
                                   {"message", "_Message"},
                                   {"stackTrace", "_StackTrace"},
                                   {"payloadPath", "_PayloadPath"}
                               });
            ContentManager.SetHandler(MediaType.ImagePng, handlerMock.Object);

            var sDataEx = new SDataException(webEx);

            ContentManager.SetHandler(MediaType.ImagePng, null);

            Assert.That(sDataEx.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(sDataEx.Diagnoses, Has.Count.EqualTo(1));
            var diagnosis = sDataEx.Diagnoses[0];
            Assert.That(diagnosis.Severity, Is.EqualTo(Severity.Error));
            Assert.That(diagnosis.SDataCode, Is.EqualTo(DiagnosisCode.BadUrlSyntax));
            Assert.That(diagnosis.ApplicationCode, Is.EqualTo("_ApplicationCode"));
            Assert.That(diagnosis.Message, Is.EqualTo("_Message"));
            Assert.That(diagnosis.StackTrace, Is.EqualTo("_StackTrace"));
            Assert.That(diagnosis.PayloadPath, Is.EqualTo("_PayloadPath"));
        }
#endif

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        [Test]
        public void Binary_Serialization_Test()
        {
            var input = new SDataException(
                new Diagnoses {new Diagnosis(Severity.Error, "test", DiagnosisCode.BadUrlSyntax, "code", "stack", "path")},
                HttpStatusCode.BadRequest);
            SDataException output;
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, input);
                stream.Seek(0, SeekOrigin.Begin);
                output = (SDataException) formatter.Deserialize(stream);
            }
            Assert.That(output.Diagnoses, Has.Count.EqualTo(1));
            Assert.That(output.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
#endif
    }
}