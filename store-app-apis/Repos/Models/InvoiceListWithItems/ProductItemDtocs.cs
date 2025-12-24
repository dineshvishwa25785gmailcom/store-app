namespace store_app_apis.Repos.Models.InvoiceListWithItems
{
    public class ProductItemDto
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? RateWithoutTax { get; set; }
        public decimal? RateWithTax { get; set; }
        public decimal? Amount { get; set; }
    }
}
