using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Metadata
{
    public enum ComplianceLevel
    {
        [XmlEnum("may")] May,
        [XmlEnum("should")] Should,
        [XmlEnum("must")] Must
    }
}