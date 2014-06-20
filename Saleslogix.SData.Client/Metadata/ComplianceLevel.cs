// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

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