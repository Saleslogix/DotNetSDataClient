// Copyright (c) Sage (UK) Limited 2007. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use
// this code. Please contact [email@sage.com] if you do not have such a licence.
// Sage will take appropriate legal action against those who make unauthorised use of this
// code.

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