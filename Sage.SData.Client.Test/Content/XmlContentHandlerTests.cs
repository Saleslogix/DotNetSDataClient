using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Sage.SData.Client.Content;
using Sage.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Content
{
    [TestFixture]
    public class XmlContentHandlerTests
    {
        [Test]
        public void Read_Tracking_Test()
        {
            const string xml = @"
                <tracking xmlns=""http://schemas.sage.com/sdata/2008/1"" >
                  <phase>Archiving FY 2007</phase>
                  <phaseDetail>Compressing file archive.dat</phaseDetail>
                  <progress>12.0</progress>
                  <elapsedSeconds>95</elapsedSeconds>
                  <remainingSeconds>568</remainingSeconds>
                  <pollingMillis>500</pollingMillis>
                </tracking>";
            object obj;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                obj = new XmlContentHandler().ReadFrom(stream);
            }
            Assert.That(obj, Is.InstanceOf<Tracking>());
            var tracking = (Tracking) obj;
            Assert.That(tracking.Phase, Is.EqualTo("Archiving FY 2007"));
            Assert.That(tracking.PhaseDetail, Is.EqualTo("Compressing file archive.dat"));
            Assert.That(tracking.Progress, Is.EqualTo(12M));
            Assert.That(tracking.ElapsedSeconds, Is.EqualTo(95M));
            Assert.That(tracking.RemainingSeconds, Is.EqualTo(568M));
            Assert.That(tracking.PollingMillis, Is.EqualTo(500));
        }

        [Test]
        public void Read_Unknown_Test()
        {
            const string xml = @"<dummy/>";
            object obj;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                obj = new XmlContentHandler().ReadFrom(stream);
            }
            Assert.That(obj, Is.InstanceOf<string>());
            Assert.That(obj, Is.EqualTo("<dummy/>"));
        }

        [Test]
        public void Write_Tracking_Test()
        {
            var tracking = new Tracking
                               {
                                   Phase = "Archiving FY 2007",
                                   PhaseDetail = "Compressing file archive.dat",
                                   Progress = 12M,
                                   ElapsedSeconds = 95M,
                                   RemainingSeconds = 568M,
                                   PollingMillis = 500
                               };
            XPathNavigator nav;
            using (var stream = new MemoryStream())
            {
                new XmlContentHandler().WriteTo(tracking, stream);
                stream.Seek(0, SeekOrigin.Begin);
                nav = new XPathDocument(stream).CreateNavigator();
            }
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            var node = nav.SelectSingleNode("sdata:tracking", mgr);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.SelectSingleNode("sdata:phase", mgr).Value, Is.EqualTo("Archiving FY 2007"));
            Assert.That(node.SelectSingleNode("sdata:phaseDetail", mgr).Value, Is.EqualTo("Compressing file archive.dat"));
            Assert.That(node.SelectSingleNode("sdata:progress", mgr).Value, Is.EqualTo("12"));
            Assert.That(node.SelectSingleNode("sdata:elapsedSeconds", mgr).Value, Is.EqualTo("95"));
            Assert.That(node.SelectSingleNode("sdata:remainingSeconds", mgr).Value, Is.EqualTo("568"));
            Assert.That(node.SelectSingleNode("sdata:pollingMillis", mgr).Value, Is.EqualTo("500"));
        }

        [Test]
        public void Write_Unknown_Test()
        {
            const string xml = @"<dummy/>";
            XPathNavigator nav;
            using (var stream = new MemoryStream())
            {
                new XmlContentHandler().WriteTo(xml, stream);
                stream.Seek(0, SeekOrigin.Begin);
                nav = new XPathDocument(stream).CreateNavigator();
            }
            var node = nav.SelectSingleNode("dummy");
            Assert.That(node, Is.Not.Null);
            Assert.That(node.IsEmptyElement, Is.True);
        }
    }
}