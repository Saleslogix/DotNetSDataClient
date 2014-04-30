using System;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDataServiceOperationAttribute : Attribute
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                IsPathSpecified = true;
            }
        }

        internal bool IsPathSpecified { get; set; }
        public string Name { get; set; }
        public string XmlLocalName { get; set; }
        public string XmlNamespace { get; set; }
        public string InstancePropertyName { get; set; }
        public string ResultPropertyName { get; set; }
    }
}