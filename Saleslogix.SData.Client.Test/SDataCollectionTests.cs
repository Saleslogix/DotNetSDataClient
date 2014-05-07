using System.Collections.Generic;
using NUnit.Framework;

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataCollectionTests
    {
        [Test]
        public void ChangeTracking_Primitive_Test()
        {
            var collection = new SDataCollection<int> {1};
            collection.AcceptChanges();
            Assert.That(collection.IsChanged, Is.False);
            Assert.That(collection.GetChanges(), Is.Null);
            collection[0] = 2;
            Assert.That(collection.IsChanged, Is.True);
            var changes = (SDataCollection<object>) collection.GetChanges();
            Assert.That(changes.DeleteMissing, Is.True);
            Assert.That(changes, Is.EqualTo(new List<object> {2}));
            collection.Add(3);
            Assert.That(collection.IsChanged, Is.True);
            Assert.That(collection.GetChanges(), Is.EqualTo(new List<object> {2, 3}));
        }

        [Test]
        public void ChangeTracking_RejectChanges_Test()
        {
            var collection = new SDataCollection<int> {1, 2};
            collection.AcceptChanges();
            Assert.That(collection.IsChanged, Is.False);
            Assert.That(collection.GetChanges(), Is.Null);
            collection.RemoveAt(0);
            collection[0] = 2;
            collection.Add(3);
            Assert.That(collection.IsChanged, Is.True);
            Assert.That(collection.GetChanges(), Is.Not.Null);
            collection.RejectChanges();
            Assert.That(collection.IsChanged, Is.False);
            Assert.That(collection.GetChanges(), Is.Null);
        }

        [Test]
        public void ChangeTracking_AddResource_Test()
        {
            var resource = new SDataResource {{"FirstName", "Joe"}, {"LastName", "Bloggs"}};
            var collection = new SDataCollection<SDataResource> {resource};
            collection.AcceptChanges();
            Assert.That(collection.IsChanged, Is.False);
            Assert.That(collection.GetChanges(), Is.Null);
            collection.Add(new SDataResource {{"FirstName", "Jim"}});
            Assert.That(collection.IsChanged, Is.True);
            var changes = (SDataCollection<object>) collection.GetChanges();
            Assert.That(changes.DeleteMissing, Is.False);
            Assert.That(changes, Is.EqualTo(new List<object> {new Dictionary<string, object> {{"FirstName", "Jim"}}}));
        }

        [Test]
        public void ChangeTracking_ChangeResource_Test()
        {
            var resource = new SDataResource {{"FirstName", "Joe"}, {"LastName", "Bloggs"}};
            var collection = new SDataCollection<SDataResource> {resource};
            collection.AcceptChanges();
            Assert.That(collection.IsChanged, Is.False);
            Assert.That(collection.GetChanges(), Is.Null);
            resource["FirstName"] = "Jill";
            Assert.That(collection.IsChanged, Is.True);
            var changes = (SDataCollection<object>) collection.GetChanges();
            Assert.That(changes.DeleteMissing, Is.False);
            Assert.That(changes, Is.EqualTo(new List<object> {new Dictionary<string, object> {{"FirstName", "Jill"}}}));
            resource["Age"] = 33;
            Assert.That(collection.IsChanged, Is.True);
            Assert.That(collection.GetChanges(), Is.EqualTo(new List<object> {new Dictionary<string, object> {{"FirstName", "Jill"}, {"Age", 33}}}));
        }

        [Test]
        public void ChangeTracking_RemoveResource_Test()
        {
            var resource = new SDataResource(new Dictionary<string, object> {{"FirstName", "Joe"}}) {Key = "abc123"};
            var collection = new SDataCollection<SDataResource> {resource};
            collection.AcceptChanges();
            Assert.That(collection.IsChanged, Is.False);
            Assert.That(collection.GetChanges(), Is.Null);
            collection.RemoveAt(0);
            Assert.That(collection.IsChanged, Is.True);
            var changes = (SDataCollection<object>) collection.GetChanges();
            Assert.That(changes.DeleteMissing, Is.False);
            Assert.That(changes, Is.EqualTo(new List<object> {new Dictionary<string, object>()}));
            resource = ((SDataResource) changes[0]);
            Assert.That(resource.Key, Is.EqualTo("abc123"));
            Assert.That(resource.IsDeleted, Is.True);
        }
    }
}