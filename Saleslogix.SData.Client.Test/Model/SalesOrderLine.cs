namespace Sage.SData.Client.Test.Model
{
    [SDataResource("salesOrderLines")]
    public class SalesOrderLine
    {
        public decimal? OrderQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public SalesOrder SalesOrder { get; set; }
        public Product Product { get; set; }
    }
}