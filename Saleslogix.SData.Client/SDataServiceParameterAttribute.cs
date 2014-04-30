using System;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SDataServiceParameterAttribute : Attribute
    {
        public string Name { get; set; }
    }
}