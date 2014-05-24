using System.Collections.Generic;

namespace Saleslogix.SData.Client.Test.Model
{
    [SDataPath("contacts")]
    public class Contact
    {
        [SDataProtocolProperty]
        public string Key { get; set; }
        public ContactCivility? Civility { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Active { get; set; }
        public Address Address { get; set; }
        public IList<Address> Addresses { get; set; }
    }

    public enum ContactCivility
    {
        Mr,
        Mrs,
        Ms
    }
}