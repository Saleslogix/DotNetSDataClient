using System.Collections.Generic;

namespace Saleslogix.SData.Client.Test.Model
{
    [SDataResource("organizations")]
    public class Organization
    {
        public string Name { get; set; }
        public IList<Contact> Contacts { get; set; }
    }
}