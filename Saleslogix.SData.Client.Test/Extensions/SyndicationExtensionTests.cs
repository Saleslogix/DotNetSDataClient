using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Extensions
{
    [TestFixture]
    public class SyndicationExtensionTests
    {
        [Test]
        public void Extension_Namespace_Declared_On_Root_Element_Test()
        {
            const string xml = @"<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:opensearch=""http://a9.com/-/spec/opensearch/1.1/"">
                                   <opensearch:startIndex>33</opensearch:startIndex>
                                 </feed>";
            var feed = Helpers.ReadAtom<SDataCollection<SDataResource>>(xml);

            Assert.That(feed.StartIndex, Is.EqualTo(33));
        }

        [Test]
        public void Extension_Namespace_Declared_On_Extension_Element_Test()
        {
            const string xml = @"<feed xmlns=""http://www.w3.org/2005/Atom"">
                                   <opensearch:startIndex xmlns:opensearch=""http://a9.com/-/spec/opensearch/1.1/"">33</opensearch:startIndex>
                                 </feed>";
            var feed = Helpers.ReadAtom<SDataCollection<SDataResource>>(xml);

            Assert.That(feed.StartIndex, Is.EqualTo(33));
        }

        [Test]
        public void Extension_Namespace_Declared_On_Extension_Element_Without_Prefix_Test()
        {
            const string xml = @"<feed xmlns=""http://www.w3.org/2005/Atom"">
                                   <startIndex xmlns=""http://a9.com/-/spec/opensearch/1.1/"">33</startIndex>
                                 </feed>";
            var feed = Helpers.ReadAtom<SDataCollection<SDataResource>>(xml);

            Assert.That(feed.StartIndex, Is.EqualTo(33));
        }
    }
}