// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    [XmlRoot(Namespace = Common.SData.Namespace)]
    [XmlType(TypeName = "diagnoses", Namespace = Common.SData.Namespace)]
    [Serializable]
    public class Diagnoses : Collection<Diagnosis>
    {
    }
}