// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Represents tracking information used to track the progress of an
    /// asynchronous operation.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = Common.SData.Namespace)]
    [XmlType(TypeName = "tracking", Namespace = Common.SData.Namespace)]
    public class Tracking
    {
        /// <summary>
        /// The current phase of the operation.
        /// </summary>
        [XmlElement("phase")]
        public string Phase { get; set; }

        /// <summary>
        /// More detailed information about the current phase of the operation.
        /// </summary>
        [XmlElement("phaseDetail")]
        public string PhaseDetail { get; set; }

        /// <summary>
        /// Percentage of the operation which is completed.
        /// </summary>
        [XmlElement("progress")]
        public decimal Progress { get; set; }

        /// <summary>
        /// Time elapsed since operation started, in seconds.
        /// </summary>
        [XmlElement("elapsedSeconds")]
        public decimal ElapsedSeconds { get; set; }

        /// <summary>
        /// Expected remaining time, in seconds
        /// </summary>
        [XmlElement("remainingSeconds")]
        public decimal RemainingSeconds { get; set; }

        /// <summary>
        /// Delay (in milliseconds) that the consumer should use 
        /// before polling the service again.
        /// </summary>
        [XmlElement("pollingMillis")]
        public int PollingMillis { get; set; }
    }
}