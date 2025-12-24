using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Repos.Models;

[Table("tbl_sales_productinfo")]
[PrimaryKey(nameof(InvoiceYear), nameof(InvoiceNumber), nameof(ProductId))] // Composite primary key
public class TblSalesProductinfo
{
    [Column("invoice_year")]
    [Unicode(false)]
    [StringLength(4)]
    public string InvoiceYear { get; set; } = null!;

    [Column("invoice_number")]
    [Unicode(false)]
    [StringLength(20)]
    public string InvoiceNumber { get; set; } = null!;

    [Column("product_id")]
    [Unicode(false)]
    [StringLength(50)]
    public string ProductId { get; set; } = null!;

    [Column("quantity")]
    [Precision(18, 3)]
    public decimal? Quantity { get; set; }

    [Column("rate_with_tax")]
    [Precision(18, 3)]
    public decimal? RateWithTax { get; set; }

    [Column("amount")]
    [Precision(18, 3)]
    public decimal? Amount { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }

    [Column("create_ip")]
    [StringLength(20)]
    public string? CreateIp { get; set; }

    [Column("update_ip")]
    [StringLength(20)]
    public string? UpdateIp { get; set; }

    [Column("create_by")]
    [Unicode(false)]
    [StringLength(20)]
    public string? CreateBy { get; set; }

    [Column("update_by")]
    [Unicode(false)]
    [StringLength(20)]
    public string? UpdateBy { get; set; }

    [Column("Rec_ID")]
    public int RecId { get; set; }

    // ✅ Navigation property with composite foreign key (InvoiceYear, InvoiceNumber)
    [ForeignKey("InvoiceYear,InvoiceNumber")]
    public TblInvoiceHeader InvoiceHeader { get; set; } = null!;

    // 📦 Navigation property to get product details from master table
    [ForeignKey(nameof(ProductId))]
    public TblProduct? Product { get; set; }
}
