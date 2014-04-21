namespace Saleslogix.SData.Client.Test.Model
{
    [SDataResource("contacts")]
    public class Contact
    {
        [SDataProtocolProperty]
        public string Key { get; set; }
        public ContactCivility? Civility { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Active { get; set; }
        public Address Address { get; set; }
    }

    public enum ContactCivility
    {
        Mr,
        Mrs,
        Ms
    }
}