using store_app_apis.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Repos.Models
{

    public class InvoiceWithCustomerDTONM
    {
        [Key]
        [Column("invoice_year")]
        public string InvoiceYear { get; set; }

        [Key]
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }

        [Column("invoice_date")]
        public DateTime? InvoiceDate { get; set; }  // ✅ Nullable DateTime

        [Column("company_id")]
        public string? CompanyId { get; set; }  // ✅ Nullable string

        [Column("customer_id")]
        public string? CustomerId { get; set; }  // ✅ Nullable string

        [Column("destination")]
        public string? Destination { get; set; }  // ✅ Nullable string

        [Column("dispatched_through")]
        public string? DispatchedThrough { get; set; }  // ✅ Nullable string

        [Column("delivery_note")]
        public string? DeliveryNote { get; set; }  // ✅ Nullable string

        [Column("remark")]
        public string? Remark { get; set; }  // ✅ Nullable string

        [Column("totalamount")]
        public decimal? TotalAmount { get; set; }  // ✅ Nullable decimal
        [Column("grand_total_amount")]
        public decimal? grand_total_amount { get; set; }  // ✅ Nullable decimal

        

        [Column("cgst_rate")]
        public decimal? CgstRate { get; set; }  // ✅ Nullable decimal

        [Column("sgst_rate")]
        public decimal? SgstRate { get; set; }  // ✅ Nullable decimal

        [Column("cgst_amount")]
        public decimal? CgstAmount { get; set; }  // ✅ Nullable decimal

        [Column("sgst_amount")]
        public decimal? SgstAmount { get; set; }  // ✅ Nullable decimal

        [Column("total_gst_amount")]
        public decimal? TotalGstAmount { get; set; }  // ✅ Nullable decimal


        public CompanyDTONM Company { get; set; }
        public TblCustomer  Customer { get; set; }
        public List<TblSalesProductinfo> Items { get; set; }
       // public List<SalesProductInfoNoDTO> Items1 { get; set; } = new List<SalesProductInfoNoDTO>();
    }
}
