using System;
using System.Reflection;
using System.Xml.Serialization;

#if !NET_2_0
using System.Runtime.Serialization;
#endif

namespace Saleslogix.SData.Client
{
    public static class NamingScheme
    {
        public static INamingScheme Default;
        public static readonly INamingScheme Basic = new BasicNamingScheme(name => name);
        public static readonly INamingScheme CamelCase = new BasicNamingScheme(name => char.IsUpper(name[0]) ? char.ToLowerInvariant(name[0]) + name.Substring(1) : name);
        public static readonly INamingScheme PascalCase = new BasicNamingScheme(name => char.IsLower(name[0]) ? char.ToUpperInvariant(name[0]) + name.Substring(1) : name);
        public static readonly INamingScheme LowerCase = new BasicNamingScheme(name => name.ToLowerInvariant());
        public static readonly INamingScheme UpperCase = new BasicNamingScheme(name => name.ToUpperInvariant());

        static NamingScheme()
        {
            Default = Basic;
        }

        #region Nested type: BasicNamingScheme

        private class BasicNamingScheme : INamingScheme
        {
            private readonly Func<string, string> _transform;

            public BasicNamingScheme(Func<string, string> transform)
            {
                _transform = transform;
            }

            public string GetName(MemberInfo member)
            {
#if !NET_2_0
                var contractAttr = member.GetCustomAttribute<DataContractAttribute>();
                if (contractAttr != null && !string.IsNullOrEmpty(contractAttr.Name))
                {
                    return contractAttr.Name;
                }

                var memberAttr = member.GetCustomAttribute<DataMemberAttribute>();
                if (memberAttr != null && !string.IsNullOrEmpty(memberAttr.Name))
                {
                    return memberAttr.Name;
                }
#endif

                var elementAttr = member.GetCustomAttribute<XmlElementAttribute>();
                if (elementAttr != null)
                {
                    return elementAttr.ElementName;
                }

                var attributeAttr = member.GetCustomAttribute<XmlAttributeAttribute>();
                if (attributeAttr != null)
                {
                    return attributeAttr.AttributeName;
                }

                var arrayAttr = member.GetCustomAttribute<XmlArrayAttribute>();
                if (arrayAttr != null)
                {
                    return arrayAttr.ElementName;
                }

                return _transform(member.Name);
            }
        }

        #endregion
    }
}