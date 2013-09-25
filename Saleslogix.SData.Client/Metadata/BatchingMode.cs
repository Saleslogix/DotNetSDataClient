﻿using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Metadata
{
    public enum BatchingMode
    {
        [XmlEnum("none")] None,
        [XmlEnum("sync")] Sync,
        [XmlEnum("async")] Async,
        [XmlEnum("syncOrAsync")] SyncOrAsync
    }
}