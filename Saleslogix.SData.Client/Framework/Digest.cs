// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Provides details of a Digest for sync.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = Common.Sync.Namespace)]
    [XmlType(TypeName = "digest", Namespace = Common.Sync.Namespace)]
    public class Digest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Digest"/> class.
        /// </summary>
        public Digest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Digest"/> class with the
        /// specified attributes.
        /// </summary>
        public Digest(string origin, params DigestEntry[] entries)
        {
            Origin = origin;
            Entries = entries;
        }

        #region Properties

        [XmlElement("origin")]
        public string Origin { get; set; }

        [XmlElement("digestEntry")]
        public DigestEntry[] Entries { get; set; }

        #endregion
    }
}