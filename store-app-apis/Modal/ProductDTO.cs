using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class ProductDTO
{
    public string? UniqueKeyID { get; set; }
    [JsonIgnore]
    public int? RecId { get; set; }
    public string? ProductName { get; set; }
    public string? Measurement { get; set; }
    public string? HsnSacNumber { get; set; }
    public string? CategoryCode { get; set; }
    public decimal? TotalGstRate { get; set; }
    public decimal? CgstRate { get; set; }
    public decimal? ScgstRate { get; set; }
    public decimal? RateWithoutTax { get; set; }
    public decimal? RateWithTax { get; set; }
    [JsonIgnore]
    public DateTime? CreateDate { get; set; }
    [JsonIgnore]
    public DateTime? UpdateDate { get; set; }
    public string? CreateIp { get; set; }
    public string? UpdateIp { get; set; }
    public string? Remark { get; set; }
    public bool? IsActive { get; set; }
}