using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace store_app_apis.Repos.Models;

[Table("tbl_Invoice")]
[PrimaryKey(nameof(InvoiceYear), nameof(InvoiceNumber))] // Composite key
public partial class TblInvoiceHeader
{
    [Column("invoice_year")]
    [MaxLength(4)]
    public string InvoiceYear { get; set; } = null!;

    [Column("invoice_number")]
    [MaxLength(20)]
    public string InvoiceNumber { get; set; } = null!;

    [Column("invoice_date")]
    public DateTime InvoiceDate { get; set; }

    [Column("company_id")]
    [MaxLength(10)]
    public string? CompanyId { get; set; }

    [Column("customer_id")]
    [MaxLength(50)]
    public string CustomerId { get; set; } = null!;

    [Column("destination")]
    [MaxLength(500)]
    public string? Destination { get; set; }

    [Column("dispatched_through")]
    [MaxLength(200)]
    public string? DispatchedThrough { get; set; }

    [Column("delivery_note")]
    [MaxLength(200)]
    public string DeliveryNote { get; set; } = null!;

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }

    [Column("create_ip")]
    [MaxLength(20)]
    public string? CreateIp { get; set; }

    [Column("update_ip")]
    [MaxLength(20)]
    public string? UpdateIp { get; set; }

    [Column("remark")]
    [MaxLength(200)]
    public string? Remark { get; set; }

    [Column("update_by")]
    [MaxLength(20)]
    public string? UpdateBy { get; set; }

    [Column("create_by")]
    [MaxLength(20)]
    public string? CreateBy { get; set; }

    [Column("totalamount")]
    [Precision(18, 2)]
    public decimal? TotalAmount { get; set; }

    [Column("Rec_Id")]
    public int RecId { get; set; }


    // 🔁 Navigation properties
    public CompanyDTONM? Company { get; set; }
    
    // 👤 Foreign key relationship to TblCustomer
    [ForeignKey(nameof(CustomerId))]
    public TblCustomer? Customer { get; set; }
    
    // ✅ One-to-Many relationship with TblSalesProductinfo
    [InverseProperty(nameof(TblSalesProductinfo.InvoiceHeader))]
    public ICollection<TblSalesProductinfo> SalesItems { get; set; } = new List<TblSalesProductinfo>();
     

}

