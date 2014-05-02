// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client
{
    public class SDataLink
    {
        public Uri Uri { get; set; }
        public string Relation { get; set; }
        public MediaType? Type { get; set; }
        public string Title { get; set; }
    }
}