namespace Saleslogix.SData.Client.Test.Model
{
    [SDataPath("addresses")]
    public class Address
    {
        [SDataProtocolProperty]
        public string Key { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
}