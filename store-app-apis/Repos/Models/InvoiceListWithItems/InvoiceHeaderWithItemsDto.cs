namespace store_app_apis.Repos.Models.InvoiceListWithItems
{
    public class InvoiceHeaderWithItemsDto
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceYear { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CgstRate { get; set; }
        public decimal? SgstRate { get; set; }
        public decimal? CgstAmount { get; set; }
        public decimal? SgstAmount { get; set; }
        public decimal? TotalGstAmount { get; set; }
        public decimal? GrandTotalAmount { get; set; }

        public string? Destination { get; set; }
        public string? DeliveryNote { get; set; }
        public string? DispatchedThrough { get; set; }
        public string? Remark { get; set; }

        public CompanyDto Company { get; set; } = new();
        public CustomerDto Customer { get; set; } = new();
        public List<ProductItemDto> Items { get; set; } = new();
    }
}
