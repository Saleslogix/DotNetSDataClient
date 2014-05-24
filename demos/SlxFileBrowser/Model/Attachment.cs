using System;
using System.ComponentModel;
using Saleslogix.SData.Client;

namespace SlxFileBrowser.Model
{
    [SDataPath("attachments")]
    public class Attachment
    {
        [SDataProtocolProperty]
        [Category("System")]
        public string Key { get; set; }

        [SDataProtocolProperty]
        [Category("System")]
        public string ETag { get; set; }

        public DateTime? AttachDate { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public string DocumentType { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string PhysicalFileName { get; set; }
        public bool FileExists { get; set; }
        public int? FileSize { get; set; }

        [Category("Links")]
        public string AccountId { get; set; }

        [Category("Links")]
        public string ContactId { get; set; }

        [Category("Links")]
        public string ContractId { get; set; }

        [Category("Links")]
        public string HistoryId { get; set; }

        [Category("Links")]
        public string LeadId { get; set; }

        [Category("Links")]
        public string OpportunityId { get; set; }

        [Category("Links")]
        public string ProductId { get; set; }

        [Category("Links")]
        public string ReturnId { get; set; }

        [Category("Links")]
        public string TicketId { get; set; }

        [Category("Links")]
        public string ActivityId { get; set; }

        [Category("Links")]
        public string SalesOrderId { get; set; }

        [Category("System")]
        public DateTime? CreateDate { get; set; }

        [Category("System")]
        public string CreateUser { get; set; }

        [Category("System")]
        public DateTime? ModifyDate { get; set; }

        [Category("System")]
        public string ModifyUser { get; set; }
    }
}