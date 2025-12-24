using System;
using System.Collections.Generic;

namespace store_app_apis.Models;
public class InvoiceItemCreateDTO
{
    public string InvoiceYear { get; set; }
    public string InvoiceNumber { get; set; }
    public string ProductId { get; set; }
    
    public string? ProductName { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? RateWithoutTax { get; set; }
    public decimal? RateWithTax { get; set; }
    public decimal? Amount { get; set; }

    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? CreateIp { get; set; }
    public string? UpdateIp { get; set; }
}