using System;
using System.Collections.Generic;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Framework
{
    internal class ContentDisposition
    {
        public static ContentDisposition Parse(string disposition)
        {
            Guard.ArgumentNotNullOrEmptyString(disposition, "disposition");

            var offset = 0;
            var type = MailBnfHelper.ReadToken(disposition, ref offset);
            if (string.IsNullOrEmpty(type))
            {
                throw new FormatException("Content disposition invalid");
            }

            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            while (MailBnfHelper.SkipCfws(disposition, ref offset))
            {
                if (disposition[offset++] != ';')
                {
                    throw new FormatException("Content disposition invalid");
                }
                if (!MailBnfHelper.SkipCfws(disposition, ref offset))
                {
                    break;
                }

                var key = MailBnfHelper.ReadParameterAttribute(disposition, ref offset);
                if (string.IsNullOrEmpty(key))
                {
                    throw new FormatException("Content disposition invalid");
                }

                if (offset >= disposition.Length || disposition[offset++] != '=')
                {
                    throw new FormatException("Content disposition invalid");
                }
                if (!MailBnfHelper.SkipCfws(disposition, ref offset))
                {
                    throw new FormatException("Content disposition invalid");
                }

                var value = disposition[offset] == '"'
                                ? MailBnfHelper.ReadQuotedString(disposition, ref offset, null)
                                : MailBnfHelper.ReadToken(disposition, ref offset);
                if (value == null)
                {
                    throw new FormatException("Content disposition invalid");
                }

                parameters.Add(key, value);
            }

            return new ContentDisposition(disposition, type, parameters);
        }

        private readonly string _disposition;
        private readonly string _type;
        private readonly IDictionary<string, string> _parameters;

        private ContentDisposition(string disposition, string type, IDictionary<string, string> parameters)
        {
            _disposition = disposition;
            _type = type;
            _parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Type
        {
            get { return _type; }
        }

        public string this[string name]
        {
            get
            {
                string value;
                _parameters.TryGetValue(name, out value);
                return value;
            }
        }

        public override string ToString()
        {
            return _disposition;
        }

        public override bool Equals(object rparam)
        {
            var disposition = rparam as ContentDisposition;
            return disposition != null && string.Equals(_disposition, disposition._disposition, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return _disposition.ToLowerInvariant().GetHashCode();
        }
    }
}