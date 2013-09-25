// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Defines the sync mode.
    /// </summary>
    public enum SyncMode
    {
        /// <summary>
        /// An alternative version of the resource.
        /// </summary>
        [XmlEnum("catchUp")] CatchUp,

        /// <summary>
        /// A resource related to this resource.
        /// </summary>
        [XmlEnum("immediate")] Immediate
    }
}