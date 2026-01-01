using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Modal
{
    public class SalesProductDTO
    {
        public int RecId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? RateWithTax { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? CreateIp { get; set; }
        public string? UpdateIp { get; set; }
        public string? UpdateBy { get; set; }
        public string? CreateBy { get; set; }

        public string? Measurement { get; set; }

        public string? HsnSacNumber { get; set; }

        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }

        
        public decimal? TotalGstRate { get; set; }

        public decimal? CgstRate { get; set; }

        public decimal? ScgstRate { get; set; }

    }
}
