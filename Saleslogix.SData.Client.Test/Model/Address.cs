namespace Saleslogix.SData.Client.Test.Model
{
    [SDataResource("addresses")]
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
}