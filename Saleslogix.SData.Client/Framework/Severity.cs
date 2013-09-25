// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Defines the severity of an error.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Informational message, does not require any special attention.
        /// </summary>
        [XmlEnum("info")] Info,

        /// <summary>
        /// Warning message: does not prevent operation from succeeding but may require attention.
        /// </summary>
        [XmlEnum("warning")] Warning,

        /// <summary>
        /// Transient error, operation failed but may succeed later in the same condition.
        /// </summary>
        [XmlEnum("transient")] Transient,

        /// <summary>
        /// Error, operation failed, request should be modified before resubmitting.
        /// </summary>
        [XmlEnum("error")] Error,

        /// <summary>
        /// Severe error, operation should not be reattempted (and other operations are likely to fail too).
        /// </summary>
        [XmlEnum("fatal")] Fatal,
    }
}