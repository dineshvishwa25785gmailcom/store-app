using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace store_app_apis.Repos.Models
{
    public class TblCustomer
    {
        [JsonIgnore] // ✅ Excludes from the request body
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Rec_Id")]
        public int Rec_Id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UniqueKeyID")]
        public string? UniqueKeyID { get; set; } // ✅ Keep UniqueKeyID but not as a primary key


        [Column("Name")]
        public string Name { get; set; }

        [Column("Email")]
        public string? Email { get; set; }  // ✅ Nullable

        [Column("Phone")]
        public string? Phone { get; set; }  // ✅ Nullable

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("address_details")]
        public string? AddressDetails { get; set; }  // ✅ Nullable

        [Column("country_code")]
        public string? CountryCode { get; set; }  // ✅ Nullable

        [Column("country_name")]
        public string? CountryName { get; set; }  // ✅ Nullable

        [Column("state_code")]
        public string? StateCode { get; set; }  // ✅ Nullable

        [Column("state_name")]
        public string? StateName { get; set; }  // ✅ Nullable

        [Column("mobile_no")]
        public string? MobileNo { get; set; }  // ✅ Nullable

        [Column("altername_mobile")]
        public string? AlternateMobile { get; set; }  // ✅ Nullable

        [Column("customer_company")]
        public string? customer_company { get; set; }  // ✅ Nullable
        [Column("gst_number")]
        public string? gst_number { get; set; }  // ✅ Nullable


        [JsonIgnore] // ✅ Excludes from the request body
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }


        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        [Column("create_ip")]
        public string? CreateIp { get; set; }

        [Column("update_ip")]
        public string? UpdateIp { get; set; }

        // ✅ Computed property (not mapped to the database)
        public string Statusname => IsActive ? "Active" : "Inactive";
    }
}
