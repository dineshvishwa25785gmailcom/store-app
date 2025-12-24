

public class Invoice_Header_DTO
{
    public int RecId { get; set; }

    public string InvoiceYear { get; set; } = null!;

    public string InvoiceNumber { get; set; } = null!;

    public DateTime? InvoiceDate { get; set; }

    public string? CompanyId { get; set; }

    public string CustomerId { get; set; } = null!;

    public string? Destination { get; set; }

    public string? DispatchedThrough { get; set; }

    public string DeliveryNote { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreateIp { get; set; }

    public string? UpdateIp { get; set; }

    public string? Remark { get; set; }

    public string? UpdateBy { get; set; }

    public string? CreateBy { get; set; }

    public decimal? Totalamount { get; set; }

    public string CompanyName { get; set; }  // ✅ New
    public string CustomerName { get; set; } // ✅ New

}