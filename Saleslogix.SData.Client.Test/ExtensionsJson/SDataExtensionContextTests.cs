using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.ExtensionsJson
{
    [TestFixture]
    public class SDataExtensionContextTests
    {
        [Test]
        public void Nested_And_Unnested_Resource_Diagnoses_Test()
        {
            const string xml = @"
                {
                  ""$diagnoses"":[
                    {
                      ""message"":""one""
                    },
                    {
                      ""message"":""two""
                    }
                  ]
                  ""$resources"":[
                    {
                      ""$diagnoses"":[
                        {
                          ""message"":""three""
                        },
                        {
                          ""message"":""four""
                        }
                      ]
                    }
                  ]
                }";
            var feed = Helpers.ReadJson<SDataCollection<SDataResource>>(xml);

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