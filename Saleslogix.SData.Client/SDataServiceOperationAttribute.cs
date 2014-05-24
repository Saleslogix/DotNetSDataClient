using System;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDataServiceOperationAttribute : Attribute
    {
        public string Name { get; set; }
        public string XmlLocalName { get; set; }
        public string XmlNamespace { get; set; }
        public string InstancePropertyName { get; set; }
        public string ResultPropertyName { get; set; }
    }
}