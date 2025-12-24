public class Sales_Productinfo_DTO
{
    public int RecId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public decimal? Quantity { get; set; }

    public decimal? RateWithoutTax { get; set; }
    public decimal? RateWithTax { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreateIp { get; set; }

    public string? UpdateIp { get; set; }

    public string? UpdateBy { get; set; }

    public string? CreateBy { get; set; }

}