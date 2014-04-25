// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace Saleslogix.SData.Client.Metadata
{
    public class SDataSchemaSimpleType : SDataSchemaValueType
    {
        private IList<XmlSchemaFacet> _facets;

        public SDataSchemaSimpleType()
        {
        }

        public SDataSchemaSimpleType(string baseName)
            : base(baseName, "type")
        {
        }

        public IList<XmlSchemaFacet> Facets
        {
            get { return _facets ?? (_facets = new List<XmlSchemaFacet>()); }
        }

        protected internal override void Read(XmlSchemaObject obj)
        {
            var simpleType = (XmlSchemaSimpleType) obj;
            var restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;

            foreach (var facet in restriction.Facets.OfType<XmlSchemaFacet>())
            {
                Facets.Add(facet);
            }

            base.Read(obj);
        }

        protected internal override void Write(XmlSchemaObject obj)
        {
            var simpleType = (XmlSchemaSimpleType) obj;
            var restriction = new XmlSchemaSimpleTypeRestriction();

            foreach (var facet in Facets)
            {
                restriction.Facets.Add(facet);
            }

            simpleType.Content = restriction;
            base.Write(obj);
        }
    }
}