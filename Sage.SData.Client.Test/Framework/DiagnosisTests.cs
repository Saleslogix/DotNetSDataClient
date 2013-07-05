using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using NUnit.Framework;
using Sage.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Framework
{
    [TestFixture]
    public class DiagnosisTests
    {
        /// <summary>
        /// According to section 3.10 of the SData spec, the sdataCode and
        /// applicationCode elements on the diagnosis type are both strings,
        /// not integers. sdataCode should be an enumeration since it has a
        /// predefined set of values.
        /// </summary>
        [Test]
        public void Diagnosis_SDataCode_Should_Be_An_Enum_Test()
        {
            var diagnosis = new Diagnosis
                                {
                                    SDataCode = DiagnosisCode.ApplicationDiagnosis,
                                    ApplicationCode = "Application error"
                                };
            string xml;

            using (var stream = new MemoryStream())
            {
                new XmlSerializer(typeof (Diagnosis)).Serialize(stream, diagnosis);
                xml = Encoding.UTF8.GetString(stream.ToArray());
            }

            XPathNavigator nav;
            using (var textReader = new StringReader(xml))
            using (var xmlReader = new XmlTextReader(textReader))
            {
                nav = new XPathDocument(xmlReader).CreateNavigator();
            }

            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("sdata", Common.SData.Namespace);

            var node = nav.SelectSingleNode("sdata:diagnosis/sdata:sdataCode", mgr);
            Assert.IsNotNull(node);
            Assert.AreEqual("ApplicationDiagnosis", node.Value);

            node = nav.SelectSingleNode("sdata:diagnosis/sdata:applicationCode", mgr);
            Assert.IsNotNull(node);
            Assert.AreEqual("Application error", node.Value);
        }

        [Test]
        public void Enum_Properties_Should_Be_Case_Insensitive_Test()
        {
            const string xml =
                @"<diagnosis xmlns=""http://schemas.sage.com/sdata/2008/1"">
                    <severity>eRrOr</severity>
                    <sdataCode>bAdUrLsYnTaX</sdataCode>
                  </diagnosis>";
            Diagnosis diagnosis;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                diagnosis = (Diagnosis) new XmlSerializer(typeof (Diagnosis)).Deserialize(stream);
            }

            Assert.That(diagnosis.Severity, Is.EqualTo(Severity.Error));
            Assert.That(diagnosis.SDataCode, Is.EqualTo(DiagnosisCode.BadUrlSyntax));
        }
    }
}