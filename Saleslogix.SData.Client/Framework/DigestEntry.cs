// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Provides details of a DigestEntry for sync.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = Common.Sync.Namespace)]
    [XmlType(TypeName = "digestEntry", Namespace = Common.Sync.Namespace)]
    public class DigestEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigestEntry"/> class.
        /// </summary>
        public DigestEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigestEntry"/> class with the
        /// specified attributes.
        /// </summary>
        public DigestEntry(string endPoint, long tick, DateTime stamp, int conflictPriority)
        {
            EndPoint = endPoint;
            Tick = tick;
            Stamp = stamp;
            ConflictPriority = conflictPriority;
        }

        #region Properties

        [XmlElement("endpoint")]
        public string EndPoint { get; set; }

        [XmlElement("tick")]
        public long Tick { get; set; }

        [XmlElement("stamp")]
        public DateTime Stamp { get; set; }

        [XmlElement("conflictPriority")]
        public int ConflictPriority { get; set; }

        #endregion
    }
}