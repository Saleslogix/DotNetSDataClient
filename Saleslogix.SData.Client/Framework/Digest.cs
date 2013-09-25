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