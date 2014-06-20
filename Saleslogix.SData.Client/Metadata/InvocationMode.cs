// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Metadata
{
    public enum InvocationMode
    {
        [XmlEnum("sync")] Sync,
        [XmlEnum("async")] Async,
        [XmlEnum("syncOrAsync")] SyncOrAsync
    }
}