// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Metadata
{
    public enum RelationshipType
    {
        [XmlEnum("parent")] Parent,
        [XmlEnum("child")] Child,
        [XmlEnum("reference")] Reference,
        [XmlEnum("association")] Association
    }
}