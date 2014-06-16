// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SDataServiceParameterAttribute : Attribute
    {
        public string Name { get; set; }
    }
}