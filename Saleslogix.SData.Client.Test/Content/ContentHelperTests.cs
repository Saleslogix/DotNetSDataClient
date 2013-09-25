using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saleslogix.SData.Client.Content;

#if !NET_2_0
using System.Runtime.Serialization;
#endif

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Content
{
    [TestFixture]
    public class ContentHelperTests
    {
        [Test]
        public void Serialize_SDataProtocolAware_Test()
        {
            var value = new SDataProtocolAware_Object {Info = new SDataProtocolInfo {XmlLocalName = "contact"}};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolAware>());
            Assert.That(((ISDataProtocolAware) result).Info.XmlLocalName, Is.EqualTo("contact"));
        }

        [Test]
        public void Deserialize_SDataProtocolAware_Test()
        {
            var value = new SDataResource {XmlLocalName = "contact"};
            var result = ContentHelper.Deserialize<SDataProtocolAware_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolAware>());
            Assert.That(result.Info.XmlLocalName, Is.EqualTo("contact"));
        }

        private class SDataProtocolAware_Object : ISDataProtocolAware
        {
            public SDataProtocolInfo Info { get; set; }
        }

        [Test]
        public void Serialize_SDataProtocolAware_Collection_Test()
        {
            var value = new SDataProtocolAware_Collection {Info = new SDataProtocolInfo {XmlLocalName = "contact"}};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolAware>());
            Assert.That(((ISDataProtocolAware) result).Info.XmlLocalName, Is.EqualTo("contact"));
        }

        [Test]
        public void Deserialize_SDataProtocolAware_Collection_Test()
        {
            var value = new SDataCollection<object> {XmlLocalName = "contact"};
            var result = ContentHelper.Deserialize<SDataProtocolAware_Collection>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Is.InstanceOf<ISDataProtocolAware>());
            Assert.That(result.Info.XmlLocalName, Is.EqualTo("contact"));
        }

        private class SDataProtocolAware_Collection : List<object>, ISDataProtocolAware
        {
            public SDataProtocolInfo Info { get; set; }
        }

#if !NET_2_0
        [Test]
        public void Serialize_DataContractAttribute_Test()
        {
            var value = new DataContractAttribute_Object();
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ISDataProtocolAware>());
            Assert.That(((ISDataProtocolAware) result).Info.XmlLocalName, Is.EqualTo("contact"));
        }

        [DataContract(Name = "contact")]
        private class DataContractAttribute_Object
        {
        }
#endif

        [Test]
        public void Serialize_Enumerable_Test()
        {
            var value = new[] {"Smith"}.Select(name => new {name});
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable>());
            Assert.That(result, Has.Count.EqualTo(1));
            var item = ((IEnumerable) result).Cast<object>().First();
            Assert.That(item, Is.InstanceOf<IDictionary<string, object>>());
            Assert.That(((IDictionary<string, object>) item)["name"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Deserialize_Enumerable_Test()
        {
            var value = new[] {"Smith"}.Select(name => new Dictionary<string, object> {{"name", name}});
            var result = ContentHelper.Deserialize<IList<IDictionary<string, object>>>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0]["name"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Serialize_SDataProtocolProperty_Test()
        {
            var value = new SDataProtocolProperty_Object {Key = "abc123"};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ISDataProtocolAware>());
            Assert.That(((ISDataProtocolAware) result).Info.Key, Is.EqualTo("abc123"));
        }

        [Test]
        public void Deserialize_SDataProtocolProperty_Test()
        {
            var value = new SDataResource {Key = "abc123"};
            var result = ContentHelper.Deserialize<SDataProtocolProperty_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result.Key, Is.EqualTo("abc123"));
        }

        private class SDataProtocolProperty_Object
        {
            [SDataProtocolProperty(SDataProtocolProperty.Key)]
            public string Key { get; set; }
        }

        [Test]
        public void Serialize_Field_Test()
        {
            var value = new Field_Object {Name = "Smith"};
            var result = ContentHelper.Serialize(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IDictionary<string, object>>());
            Assert.That(((IDictionary<string, object>) result)["Name"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Deserialize_Field_Test()
        {
            var value = new Dictionary<string, object> {{"Name", "Smith"}};
            var result = ContentHelper.Deserialize<Field_Object>(value);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(value));
            Assert.That(result.Name, Is.EqualTo("Smith"));
        }

        private class Field_Object
        {
            public string Name;
        }
    }
}