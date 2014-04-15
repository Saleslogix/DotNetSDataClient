using NUnit.Framework;

namespace Saleslogix.SData.Client.Test
{
    [TestFixture]
    public class SDataResourceTests
    {
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
    }
}