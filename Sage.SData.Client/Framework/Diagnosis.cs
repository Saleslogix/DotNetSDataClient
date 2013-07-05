// Copyright (c) Sage (UK) Limited 2007. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use
// this code. Please contact [email@sage.com] if you do not have such a licence.
// Sage will take appropriate legal action against those who make unauthorised use of this
// code.

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// Provides details of an error that has occurred.
    /// </summary>
    [XmlRoot(Namespace = Common.SData.Namespace)]
    [XmlType(TypeName = "diagnosis", Namespace = Common.SData.Namespace)]
    [Serializable]
    public class Diagnosis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Diagnosis"/> class.
        /// </summary>
        public Diagnosis()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Diagnosis"/> class.
        /// </summary>
        /// <param name="severity">One of the <see cref="Severity"/> values.</param>
        /// <param name="message">A friendly message for the diagnosis.</param>
        /// <param name="code">One of the <see cref="DiagnosisCode"/> values.</param>
        public Diagnosis(Severity severity, string message, DiagnosisCode code) :
            this(severity, message, code, string.Empty)
        {
        }

        public Diagnosis(Severity severity, string message, DiagnosisCode code, string applicationCode) :
            this(severity, message, code, applicationCode, string.Empty)
        {
        }

        public Diagnosis(Severity severity, string message, DiagnosisCode code, string applicationCode, string stackTrace) :
            this(severity, message, code, applicationCode, stackTrace, string.Empty)
        {
        }

        public Diagnosis(Severity severity, string message, DiagnosisCode code, string applicationCode, string stackTrace, string payloadPath)
        {
            Severity = severity;
            SDataCode = code;
            ApplicationCode = applicationCode;
            Message = message;
            StackTrace = stackTrace;
            PayloadPath = payloadPath;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Severity"/> of the error.
        /// </summary>
        /// <value>One of the <see cref="Severity"/> values.</value>
        [XmlIgnore]
        public Severity? Severity { get; set; }

        [XmlElement("severity")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SeverityString
        {
            get { return Severity != null ? Severity.Value.ToString() : null; }
            set { Severity = value != null ? (Severity) Enum.Parse(typeof (Severity), value, true) : (Severity?) null; }
        }

        /// <summary>
        /// Gets or sets the SData diagnosis code for the error.
        /// </summary>
        /// <value>An SData diagnosis code for the error.</value>
        [XmlIgnore]
        public DiagnosisCode? SDataCode { get; set; }

        [XmlElement("sdataCode")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SDataCodeString
        {
            get { return SDataCode != null ? SDataCode.Value.ToString() : null; }
            set { SDataCode = value != null ? (DiagnosisCode) Enum.Parse(typeof (DiagnosisCode), value, true) : (DiagnosisCode?) null; }
        }

        /// <summary>
        /// Gets or sets the application specific diagnosis code for the error.
        /// </summary>
        /// <value>An application specific diagnosis code for the error.</value>
        [XmlElement("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets a friendly message for the diagnosis.
        /// </summary>
        /// <value>A user friendly description of the error.</value>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace for the error.
        /// </summary>
        /// <value>A stack trace of the error.</value>
        [XmlElement("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// XPath expression that designates the payload element which is responsible for the error
        /// </summary>
        /// <value>An XPath expression</value>
        [XmlElement("payloadPath")]
        public string PayloadPath { get; set; }

        #endregion
    }
}