using System.Linq;
using System.Xml.XPath;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.Extensions
{
    [TestFixture]
    public class SDataSimpleCollectionTests
    {
        #region deserialization tests

        private const string TestCase1 = @"
  <entry xmlns=""http://www.w3.org/2005/Atom"">
    <sdata:payload xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
      <validationRule
            sdata:key=""fc9bd0aee4d0445395f69dbc3070b6a1""
            sdata:uri=""http://localhost:8001/sdata/$app/metadata/-/validationRules('fc9bd0aee4d0445395f69dbc3070b6a1')""
            sdata:lookup=""http://localhost:8001/sdata/$app/metadata/-/validationRules""
            xmlns=""http://schemas.sage.com/gobiplatform/2010""
            xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
        <name>AccountTypeValidation</name>
        <displayName>Account Type Validation</displayName>
        <entity sdata:key=""Account"" sdata:uri=""http://localhost:8001/sdata/$app/metadata/-/entities('Account')"" sdata:lookup=""http://localhost:8001/sdata/$app/metadata/-/entities"" />
        <isPropertyLevel>true</isPropertyLevel>
        <propertyName>Type</propertyName>
        <customMessageTemplate>false</customMessageTemplate>
        <messageTemplate xsi:nil=""true"" />
        <validatorType>
          <listValidator>
            <items>
              <item>Customer</item>
              <item>Prospect</item>
              <item>Lead</item>
              <item>None</item>
            </items>
            <operator>In</operator>
          </listValidator>
        </validatorType>
      </validationRule>
    </sdata:payload>
  </entry>";

        [Test]
        public void correctly_identifies_simple_collection()
        {
            var payload = Helpers.ReadAtom<SDataResource>(TestCase1);

            var validatorType = (SDataResource) payload["validatorType"];
            var listValidator = (SDataResource) validatorType["listValidator"];
            Assert.That(listValidator["items"], Is.InstanceOf<SDataCollection<object>>());
        }

        [Test]
        public void correct_number_of_items_in_simple_collection()
        {
            var payload = Helpers.ReadAtom<SDataResource>(TestCase1);

            var validatorType = (SDataResource) payload["validatorType"];
            var listValidator = (SDataResource) validatorType["listValidator"];
            Assume.That(listValidator["items"], Is.InstanceOf<SDataCollection<object>>());

            var items = (SDataCollection<object>) listValidator["items"];
            Assert.That(items, Has.Count.EqualTo(4));
        }

        [Test]
        public void correct_values_in_simple_collection()
        {
            var payload = Helpers.ReadAtom<SDataResource>(TestCase1);

            var validatorType = (SDataResource) payload["validatorType"];
            var listValidator = (SDataResource) validatorType["listValidator"];
            Assume.That(listValidator["items"], Is.InstanceOf<SDataCollection<object>>());

            var items = (SDataCollection<object>) listValidator["items"];
            var res = items.Intersect(ItemValues).Count();
            Assert.That(res, Is.EqualTo(4));
        }

        #endregion

        #region serialization tests

        private readonly SDataResource SerializationTestCase1 =
            new SDataResource("validationRule")
                {
                    {
                        "validatorType",
                        new SDataCollection<SDataResource>
                            {
                                new SDataResource("listValidator")
                                    {
                                        {"items", new SDataCollection<object>("item") {"Customer", "Prospect", "Lead", "None"}}
                                    }
                            }
                    }
                };

        private readonly string[] ItemValues = new[] {"Customer", "Prospect", "Lead", "None"};

        [Test]
        public void does_not_throw_exception_when_ItemElementName_is_set()
        {
            var res = Helpers.WriteAtom(SerializationTestCase1);
            Assert.That(res, Is.InstanceOf<XPathNavigator>());
        }

        [Test]
        public void correctly_serializes_array_element_name()
        {
            var res = Helpers.WriteAtom(SerializationTestCase1);
            var items = res.SelectSingleNode("//items");
            Assert.That(items, Is.Not.Null);
        }

        [Test]
        public void correct_number_of_items_serialized_in_array()
        {
            var res = Helpers.WriteAtom(SerializationTestCase1);
            var items = res.Select("//items/item");
            Assert.That(items.Count, Is.EqualTo(4));
        }

        [Test]
        public void correct_values_serialized_in_array()
        {
            var res = Helpers.WriteAtom(SerializationTestCase1);
            var itemIter = res.Select("//items/item");
            var count = itemIter.Cast<XPathNavigator>().Select(x => x.Value).Intersect(ItemValues).Count();
            Assert.That(count, Is.EqualTo(4));
        }

        private readonly SDataResource SerializationTestCase2 =
            new SDataResource("validationRule")
                {
                    {
                        "validatorType",
                        new SDataCollection<SDataResource>
                            {
                                new SDataResource("listValidator")
                                    {
                                        {"items", new SDataCollection<object>("item")}
                                    }
                            }
                    }
                };

        [Test]
        public void correctly_serializes_empty_array()
        {
            var res = Helpers.WriteAtom(SerializationTestCase2);
            var items = res.SelectSingleNode("//items");

            Assert.That(items, Is.Not.Null);
            Assert.That(items.HasChildren, Is.False);
        }

        #endregion
    }
}