using System.Text.Json.Serialization;

namespace store_app_apis.Modal
{
    public class MeasurementDto
    {
        [JsonIgnore] // Excludes from the request body
        public int Rec_Id { get; set; }
        public string? UniqueKeyID { get; set; }
        public string? Name { get; set; }

        [JsonIgnore] // Excludes from the request body
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? CreateIp { get; set; }
        public string? UpdateIp { get; set; }
        public bool? IsActive { get; set; }
    }
}
