using System.ComponentModel;
using NUnit.Framework;

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataCollectionTypeConverterTests
    {
        [Test]
        public void ToString_Test()
        {
            var resources = new SDataCollection<SDataResource> {new SDataResource {Descriptor = "Widget"}};
            var converter = TypeDescriptor.GetConverter(resources);
            Assert.That(converter.CanConvertTo(typeof (string)), Is.True);
            var value = converter.ConvertTo(resources, typeof (string));
            Assert.That(value, Is.EqualTo("(1 item)"));
        }

        [Test]
        public void Properties_Test()
        {
            var resources = new SDataCollection<SDataResource> {new SDataResource {Descriptor = "Widget"}};
            var converter = TypeDescriptor.GetConverter(resources);
            var props = converter.GetProperties(resources);
            Assert.That(props, Is.Not.Null);
            Assert.That(props.Count, Is.EqualTo(1));
            var prop = props[0];
            Assert.That(prop.Name, Is.EqualTo("[0]"));
        }
    }
}