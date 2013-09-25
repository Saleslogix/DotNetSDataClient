using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Saleslogix.SData.Client.Test.ExtensionsJson
{
    [TestFixture]
    public class SDataSimpleCollectionTests
    {
        #region deserialization tests

        private const string TestCase1 = @"
            {
              ""$key"":""fc9bd0aee4d0445395f69dbc3070b6a1"",
              ""$url"":""http://localhost:8001/sdata/$app/metadata/-/validationRules('fc9bd0aee4d0445395f69dbc3070b6a1')"",
              ""$lookup"":""http://localhost:8001/sdata/$app/metadata/-/validationRules"",
              ""name"":""AccountTypeValidation"",
              ""displayName"":""Account Type Validation"",
              ""entity"":{
                ""$key"":""Account"",
                ""$url"":""http://localhost:8001/sdata/$app/metadata/-/entities('Account')"",
                ""$lookup"":""http://localhost:8001/sdata/$app/metadata/-/entities""
              },
              ""isPropertyLevel"":""true"",
              ""propertyName"":""Type"",
              ""customMessageTemplate"":""false"",
              ""messageTemplate"":null,
              ""validatorType"":{
                ""listValidator"":{
                  ""items"":[
                    ""Customer"",
                    ""Prospect"",
                    ""Lead"",
                    ""None""
                  ],
                  ""operator"":""In""
                }
              }
            }";

        [Test]
        public void correctly_identifies_simple_collection()
        {
            var payload = Helpers.ReadJson<SDataResource>(TestCase1);

            var validatorType = (SDataResource) payload["validatorType"];
            var listValidator = (SDataResource) validatorType["listValidator"];
            Assert.That(listValidator["items"], Is.InstanceOf<SDataCollection<object>>());
        }

        [Test]
        public void correct_number_of_items_in_simple_collection()
        {
            var payload = Helpers.ReadJson<SDataResource>(TestCase1);

            var validatorType = (SDataResource) payload["validatorType"];
            var listValidator = (SDataResource) validatorType["listValidator"];
            Assume.That(listValidator["items"], Is.InstanceOf<SDataCollection<object>>());

            var items = (SDataCollection<object>) listValidator["items"];
            Assert.That(items, Has.Count.EqualTo(4));
        }

        [Test]
        public void correct_values_in_simple_collection()
        {
            var payload = Helpers.ReadJson<SDataResource>(TestCase1);

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
            new SDataResource
                {
                    {
                        "validatorType",
                        new SDataCollection<SDataResource>
                            {
                                new SDataResource
                                    {
                                        {"items", new SDataCollection<object> {"Customer", "Prospect", "Lead", "None"}}
                                    }
                            }
                    }
                };

        private readonly string[] ItemValues = new[] {"Customer", "Prospect", "Lead", "None"};

        [Test]
        public void does_not_throw_exception_when_ItemElementName_is_set()
        {
            var res = Helpers.WriteJson(SerializationTestCase1);
            Assert.That(res, Is.Not.Null);
        }

        [Test]
        public void correctly_serializes_array_element_name()
        {
            var res = Helpers.WriteJson(SerializationTestCase1);
            Assume.That(res["validatorType"], Is.InstanceOf<IDictionary<string, object>>());
            var validatorType = (IDictionary<string, object>) res["validatorType"];
            Assume.That(validatorType["$resources"], Is.InstanceOf<IList<object>>());
            var resources = (IList<object>) validatorType["$resources"];
            Assume.That(resources[0], Is.InstanceOf<IDictionary<string, object>>());
            var resource = (IDictionary<string, object>) resources[0];
            Assert.That(resource["items"], Is.InstanceOf<IList<object>>());
        }

        [Test]
        public void correct_number_of_items_serialized_in_array()
        {
            var res = Helpers.WriteJson(SerializationTestCase1);
            Assume.That(res["validatorType"], Is.InstanceOf<IDictionary<string, object>>());
            var validatorType = (IDictionary<string, object>) res["validatorType"];
            Assume.That(validatorType["$resources"], Is.InstanceOf<IList<object>>());
            var resources = (IList<object>) validatorType["$resources"];
            Assume.That(resources[0], Is.InstanceOf<IDictionary<string, object>>());
            var resource = (IDictionary<string, object>) resources[0];
            Assume.That(resource["items"], Is.InstanceOf<IList<object>>());
            var items = (IList<object>) resource["items"];
            Assert.That(items.Count, Is.EqualTo(4));
        }

        [Test]
        public void correct_values_serialized_in_array()
        {
            var res = Helpers.WriteJson(SerializationTestCase1);
            Assume.That(res["validatorType"], Is.InstanceOf<IDictionary<string, object>>());
            var validatorType = (IDictionary<string, object>) res["validatorType"];
            Assume.That(validatorType["$resources"], Is.InstanceOf<IList<object>>());
            var resources = (IList<object>) validatorType["$resources"];
            Assume.That(resources[0], Is.InstanceOf<IDictionary<string, object>>());
            var resource = (IDictionary<string, object>) resources[0];
            Assume.That(resource["items"], Is.InstanceOf<IList<object>>());
            var items = (IList<object>) resource["items"];
            Assert.That(items, Is.EquivalentTo(ItemValues));
        }

        private readonly SDataResource SerializationTestCase2 =
            new SDataResource
                {
                    {
                        "validatorType",
                        new SDataCollection<SDataResource>
                            {
                                new SDataResource
                                    {
                                        {"items", new SDataCollection<object>()}
                                    }
                            }
                    }
                };

        [Test]
        public void correctly_serializes_empty_array()
        {
            var res = Helpers.WriteJson(SerializationTestCase2);
            Assume.That(res["validatorType"], Is.InstanceOf<IDictionary<string, object>>());
            var validatorType = (IDictionary<string, object>) res["validatorType"];
            Assume.That(validatorType["$resources"], Is.InstanceOf<IList<object>>());
            var resources = (IList<object>) validatorType["$resources"];
            Assume.That(resources[0], Is.InstanceOf<IDictionary<string, object>>());
            var resource = (IDictionary<string, object>) resources[0];
            Assume.That(resource["items"], Is.InstanceOf<IList<object>>());
            Assert.That(resource["items"], Is.Empty);
        }

        #endregion
    }
}