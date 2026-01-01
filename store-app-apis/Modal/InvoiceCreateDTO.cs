using store_app_apis.Models;
using store_app_apis.Repos.Models;
public class InvoiceCreateDTO
{
    public string InvoiceYear { get; set; } = null!;
    public string InvoiceNumber { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }

    public string? CompanyId { get; set; }
    public string CustomerId { get; set; } = null!;

    public string? Destination { get; set; }
    public string? DispatchedThrough { get; set; }
    public string DeliveryNote { get; set; } = null!;
    public string? Remark { get; set; }

    public decimal? TotalAmount { get; set; }
    public decimal? GrandTotalAmount { get; set; }

    public decimal? CgstRate { get; set; }
    public decimal? SgstRate { get; set; }
    public decimal? CgstAmount { get; set; }
    public decimal? SgstAmount { get; set; }
    public decimal? TotalGstAmount { get; set; }

    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? CreateIp { get; set; }
    public string? UpdateIp { get; set; }

    public List<InvoiceItemCreateDTO> Products { get; set; } = new();
}