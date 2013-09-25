// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Framework
{
    internal class ContentType
    {
        private readonly string _type;
        private readonly string _subType;
        private readonly string _mediaType;
        private readonly IDictionary<string, string> _parameters;

        public ContentType(string type)
        {
            Guard.ArgumentNotNullOrEmptyString(type, "type");
            _type = type;

            var offset = 0;
            _mediaType = MailBnfHelper.ReadToken(_type, ref offset);
            if (string.IsNullOrEmpty(_mediaType))
            {
                throw new FormatException("Content type invalid");
            }
            offset++;
            _subType = MailBnfHelper.ReadToken(_type, ref offset);
            if (string.IsNullOrEmpty(_subType))
            {
                throw new FormatException("Content type invalid");
            }
            _parameters = Parse(type, offset);
        }

        public string MediaType
        {
            get { return _mediaType; }
        }

        public string SubType
        {
            get { return _subType; }
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

        private static IDictionary<string, string> Parse(string type, int offset)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            while (MailBnfHelper.SkipCfws(type, ref offset))
            {
                if (type[offset++] != ';')
                {
                    throw new FormatException("Content type invalid");
                }
                if (!MailBnfHelper.SkipCfws(type, ref offset))
                {
                    break;
                }

                var key = MailBnfHelper.ReadParameterAttribute(type, ref offset);
                if (string.IsNullOrEmpty(key))
                {
                    throw new FormatException("Content type invalid");
                }

                if (offset >= type.Length || type[offset++] != '=')
                {
                    throw new FormatException("Content type invalid");
                }
                if (!MailBnfHelper.SkipCfws(type, ref offset))
                {
                    throw new FormatException("Content type invalid");
                }

                var value = type[offset] == '"'
                                ? MailBnfHelper.ReadQuotedString(type, ref offset, null)
                                : MailBnfHelper.ReadToken(type, ref offset);
                if (value == null)
                {
                    throw new FormatException("Content type invalid");
                }

                parameters.Add(key, value);
            }

            return parameters;
        }

        public override string ToString()
        {
            return _type;
        }

        public override bool Equals(object rparam)
        {
            var type = rparam as ContentType;
            return type != null && string.Equals(_type, type._type, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return _type.ToLowerInvariant().GetHashCode();
        }
    }
}