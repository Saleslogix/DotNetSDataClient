using System;
using System.Collections.Generic;

namespace Saleslogix.SData.Client.Test.Model
{
    [SDataPath("salesOrders")]
    public class SalesOrder
    {
        public string OrderNumber { get; set; }
        public DateTimeOffset? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public decimal? SubTotal { get; set; }
        public int? LineCount { get; set; }
        public Address BillAddress { get; set; }
        public Address ShipAddress { get; set; }
        public IList<SalesOrderLine> OrderLines { get; set; }
        public Contact Contact { get; set; }
    }
}