using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Extensions
{
    [TestFixture]
    public class SDataExtensionContextTests
    {
        [Test]
        public void Nested_And_Unnested_Resource_Diagnoses_Test()
        {
            const string xml = @"
<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
  <sdata:diagnoses>
    <sdata:diagnosis>
      <sdata:message>one</sdata:message>
    </sdata:diagnosis>
  </sdata:diagnoses>
  <sdata:diagnosis>
    <sdata:message>two</sdata:message>
  </sdata:diagnosis>
  <entry>
    <sdata:diagnoses>
      <sdata:diagnosis>
        <sdata:message>three</sdata:message>
      </sdata:diagnosis>
    </sdata:diagnoses>
    <sdata:diagnosis>
      <sdata:message>four</sdata:message>
    </sdata:diagnosis>
  </entry>
</feed>";
            var feed = Helpers.ReadAtom<SDataCollection<SDataResource>>(xml);

            var diagnoses = feed.Diagnoses;
            Assert.That(diagnoses.Count, Is.EqualTo(2));
            Assert.That(diagnoses[0].Message, Is.EqualTo("one"));
            Assert.That(diagnoses[1].Message, Is.EqualTo("two"));

            Assume.That(feed.Any());
            diagnoses = feed.First().Diagnoses;
            Assert.That(diagnoses.Count, Is.EqualTo(2));
            Assert.That(diagnoses[0].Message, Is.EqualTo("three"));
            Assert.That(diagnoses[1].Message, Is.EqualTo("four"));
        }
    }
}