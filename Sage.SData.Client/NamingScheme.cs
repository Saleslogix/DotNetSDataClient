using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sage.SData.Client
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
                var contractAttr = (DataContractAttribute) Attribute.GetCustomAttribute(member, typeof (DataContractAttribute));
                if (contractAttr != null && !string.IsNullOrEmpty(contractAttr.Name))
                {
                    return contractAttr.Name;
                }

                var memberAttr = (DataMemberAttribute) Attribute.GetCustomAttribute(member, typeof (DataMemberAttribute));
                if (memberAttr != null && !string.IsNullOrEmpty(memberAttr.Name))
                {
                    return memberAttr.Name;
                }
#endif

                var elementAttr = (XmlElementAttribute) Attribute.GetCustomAttribute(member, typeof (XmlElementAttribute));
                if (elementAttr != null)
                {
                    return elementAttr.ElementName;
                }

                var attributeAttr = (XmlAttributeAttribute) Attribute.GetCustomAttribute(member, typeof (XmlAttributeAttribute));
                if (attributeAttr != null)
                {
                    return attributeAttr.AttributeName;
                }

                var arrayAttr = (XmlArrayAttribute) Attribute.GetCustomAttribute(member, typeof (XmlArrayAttribute));
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