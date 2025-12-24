using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Repos.Models
{
    public class SalesProductInfoDTONM
    {
        [Key]
        [Column("invoice_number", Order = 1)]
        public string InvoiceNumber { get; set; }

        [Key]
        [Column("product_id", Order = 2)]
        public string ProductId { get; set; }  // ✅ FK linking to Tbl_product

        [Column("quantity")]
        public decimal? Quantity { get; set; }

        [Column("rate_without_tax")]
        public decimal? RateWithoutTax { get; set; }

        [Column("rate_with_tax")]
        public decimal? RateWithTax { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        // ✅ Foreign Key Navigation Property
        public ProductDTONM Product { get; set; }
    }
}