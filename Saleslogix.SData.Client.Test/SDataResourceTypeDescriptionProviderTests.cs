using System.ComponentModel;
using NUnit.Framework;

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataResourceTypeDescriptionProviderTests
    {
        [Test]
        public void Type_Test()
        {
            var props = TypeDescriptor.GetProperties(typeof (SDataResource));
            Assert.That(props, Is.Empty);
        }

        [Test]
        public void Get_Primitive_Test()
        {
            var resource = new SDataResource {{"LastName", "Smith"}};
            var prop = TypeDescriptor.GetProperties(resource)["LastName"];
            Assert.That(prop, Is.Not.Null);
            Assert.That(prop.PropertyType, Is.EqualTo(typeof (string)));
            Assert.That(prop.GetValue(resource), Is.EqualTo("Smith"));
        }

        [Test]
        public void Set_Primitive_Test()
        {
            var resource = new SDataResource {{"LastName", null}};
            var prop = TypeDescriptor.GetProperties(resource)["LastName"];
            Assert.That(prop, Is.Not.Null);
            prop.SetValue(resource, "Smith");
            Assert.That(resource["LastName"], Is.EqualTo("Smith"));
        }

        [Test]
        public void Get_NestedResource_Test()
        {
            var resource = new SDataResource {{"Address", new SDataResource()}};
            var prop = TypeDescriptor.GetProperties(resource)["Address"];
            Assert.That(prop, Is.Not.Null);
            Assert.That(prop.PropertyType, Is.EqualTo(typeof (SDataResource)));
            Assert.That(prop.Converter, Is.TypeOf<ExpandableObjectConverter>());
        }

        [Test]
        public void Get_NestedResourceCollection_Test()
        {
            var resource = new SDataResource {{"OrderLines", new SDataCollection<SDataResource>()}};
            var prop = TypeDescriptor.GetProperties(resource)["OrderLines"];
            Assert.That(prop, Is.Not.Null);
            Assert.That(prop.PropertyType, Is.EqualTo(typeof (SDataCollection<SDataResource>)));
            Assert.That(prop.Converter, Is.Not.Null);
            Assert.That(prop.Converter.GetPropertiesSupported(), Is.True);
        }
    }
}