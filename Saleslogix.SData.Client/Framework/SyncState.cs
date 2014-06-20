// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Provides details of a SyncState for sync.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = Common.Sync.Namespace)]
    [XmlType(TypeName = "syncState", Namespace = Common.Sync.Namespace)]
    public class SyncState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncState"/> class.
        /// </summary>
        public SyncState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncState"/> class with the
        /// specified attributes.
        /// </summary>
        public SyncState(string endPoint, long tick, DateTime stamp)
        {
            EndPoint = endPoint;
            Tick = tick;
            Stamp = stamp;
        }

        #region Properties

        [XmlElement("endpoint")]
        public string EndPoint { get; set; }

        [XmlElement("tick")]
        public long Tick { get; set; }

        [XmlElement("stamp")]
        public DateTime Stamp { get; set; }

        #endregion
    }
}