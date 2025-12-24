using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace store_app_apis.Repos.Models
{
    public class CompanyNoDTO
    {
        [Key]
        [Column("CompanyId")]
        public string CompanyId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("address_details")]
        public string? AddressDetails { get; set; }  // ✅ Nullable

        [Column("gst_number")]
        public string? GstNumber { get; set; }  // ✅ Nullable

        [Column("country_code")]
        public string? CountryCode { get; set; }  // ✅ Nullable

        [Column("country_name")]
        public string? CountryName { get; set; }  // ✅ Nullable

        [Column("state_code")]
        public string? StateCode { get; set; }  // ✅ Nullable

        [Column("state_name")]
        public string? StateName { get; set; }  // ✅ Nullable

        [Column("account_number")]
        public string? AccountNumber { get; set; }  // ✅ Nullable

        [Column("ifsc")]
        public string? Ifsc { get; set; }  // ✅ Nullable

        [Column("account_address")]
        public string? AccountAddress { get; set; }  // ✅ Nullable

        [Column("email_id")]
        public string? EmailId { get; set; }  // ✅ Nullable

        [Column("mobile_no")]
        public string? MobileNo { get; set; }  // ✅ Nullable

        [Column("altername_mobile")]
        public string? AlternateMobile { get; set; }  // ✅ Nullable

        [Column("UniqueKeyID")]
        public string? UniqueKeyID { get; set; }  // ✅ Nullable

        [Column("Status")]
        public string? Status { get; set; }  // ✅ Nullable
    }
}
