using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class CategoryDTO
{
    [JsonIgnore] // Excludes from the request body
    public int RecId { get; set; }
    [JsonIgnore] // Excludes from the request body
    public DateTime? CreateDate { get; set; }

    public string? Name { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? CreateIp { get; set; }
    public string? UpdateIp { get; set; }
    public string? UniqueKeyId { get; set; }
    public bool? IsActive { get; set; }
}