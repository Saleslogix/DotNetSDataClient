using System.Collections.Generic;
using NUnit.Framework;

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataResourceTests
    {
#if !NET_2_0
        [Test]
        public void Dynamic_Get_Property_Test()
        {
            var resource = new SDataResource {{"Name", "Joe"}};
            dynamic obj = resource;
            Assert.That(obj.Name, Is.EqualTo("Joe"));
        }

        [Test]
        public void Dynamic_Get_Indexer_Test()
        {
            var resource = new SDataResource {{"Name", "Joe"}};
            dynamic obj = resource;
            Assert.That(obj["Name"], Is.EqualTo("Joe"));
        }

        [Test]
        public void Dynamic_Set_Property_Test()
        {
            var resource = new SDataResource();
            dynamic obj = resource;
            obj.Name = "Joe";
            Assert.That(resource["Name"], Is.EqualTo("Joe"));
        }

        [Test]
        public void Dynamic_Set_Indexer_Test()
        {
            var resource = new SDataResource();
            dynamic obj = resource;
            obj["Name"] = "Joe";
            Assert.That(resource["Name"], Is.EqualTo("Joe"));
        }
#endif

        [Test]
        public void ChangeTracking_Test()
        {
            var resource = new SDataResource {{"FirstName", "Joe"}, {"LastName", "Bloggs"}};
            resource.AcceptChanges();
            Assert.That(resource.IsChanged, Is.False);
            Assert.That(resource.GetChanges(), Is.Null);
            resource["FirstName"] = "Jill";
            Assert.That(resource.IsChanged, Is.True);
            Assert.That(resource.GetChanges(), Is.EqualTo(new Dictionary<string, object> {{"FirstName", "Jill"}}));
            resource.Add("Age", 33);
            Assert.That(resource.IsChanged, Is.True);
            Assert.That(resource.GetChanges(), Is.EqualTo(new Dictionary<string, object> {{"FirstName", "Jill"}, {"Age", 33}}));
        }

        [Test]
        public void ChangeTracking_RejectChanges_Test()
        {
            var resource = new SDataResource {{"FirstName", "Joe"}, {"LastName", "Bloggs"}};
            resource.AcceptChanges();
            Assert.That(resource.IsChanged, Is.False);
            Assert.That(resource.GetChanges(), Is.Null);
            resource.Remove("LastName");
            resource["FirstName"] = "Jill";
            resource.Add("Age", 33);
            Assert.That(resource.IsChanged, Is.True);
            Assert.That(resource.GetChanges(), Is.Not.Null);
            resource.RejectChanges();
            Assert.That(resource.IsChanged, Is.False);
            Assert.That(resource.GetChanges(), Is.Null);
        }

        [Test]
        public void ChangeTracking_Nested_Test()
        {
            var nested = new SDataResource {{"Country", "Australia"}, {"City", "Melbourne"}};
            var resource = new SDataResource {{"Address", nested}};
            resource.AcceptChanges();
            Assert.That(nested.IsChanged, Is.False);
            Assert.That(nested.GetChanges(), Is.Null);
            Assert.That(resource.IsChanged, Is.False);
            Assert.That(resource.GetChanges(), Is.Null);
            nested["City"] = "Sydney";
            Assert.That(nested.IsChanged, Is.True);
            Assert.That(nested.GetChanges(), Is.EqualTo(new Dictionary<string, object> {{"City", "Sydney"}}));
            Assert.That(resource.IsChanged, Is.True);
            var changes = (IDictionary<string, object>) resource.GetChanges();
            Assert.That(changes.Count, Is.EqualTo(1));
            Assert.That(changes["Address"], Is.EqualTo(new Dictionary<string, object> {{"City", "Sydney"}}));
            nested.Add("State", "VIC");
            Assert.That(nested.IsChanged, Is.True);
            Assert.That(nested.GetChanges(), Is.EqualTo(new Dictionary<string, object> {{"City", "Sydney"}, {"State", "VIC"}}));
            Assert.That(resource.IsChanged, Is.True);
            changes = (IDictionary<string, object>) resource.GetChanges();
            Assert.That(changes.Count, Is.EqualTo(1));
            Assert.That(changes["Address"], Is.EqualTo(new Dictionary<string, object> {{"City", "Sydney"}, {"State", "VIC"}}));
        }

        [Test]
        public void ChangeTracking_Retain_Key_And_ETag_Test()
        {
            var nested = new SDataResource {{"Country", "Australia"}, {"City", "Melbourne"}};
            nested.Key = "address1";
            nested.ETag = "aaa1";
            var resource = new SDataResource {{"FirstName", "Joe"}, {"Address", nested}};
            resource.Key = "contact1";
            resource.ETag = "ccc1";
            resource.AcceptChanges();
            resource["FirstName"] = "Joanne";
            nested["City"] = "Sydney";
            var changes = (SDataResource) resource.GetChanges();
            Assert.That(changes, Is.Not.EqualTo(resource));
            Assert.That(changes.Key, Is.EqualTo("contact1"));
            Assert.That(changes.ETag, Is.EqualTo("ccc1"));
            changes = (SDataResource) changes["Address"];
            Assert.That(changes, Is.Not.EqualTo(nested));
            Assert.That(changes.Key, Is.EqualTo("address1"));
            Assert.That(changes.ETag, Is.EqualTo("aaa1"));
        }
    }
}