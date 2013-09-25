using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Framework
{
    [TestFixture]
    public class SDataExceptionTests
    {
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
            Assert.That(output.StatusCode == input.StatusCode);
        }
    }
}