using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace store_app_apis.Modal
{
    public class CustomerDTO
    {
        public int RecId { get; set; }

        public string Name { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }

      

        public bool? IsActive { get; set; }

         

        public string? AddressDetails { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? CreateIp { get; set; }

        public string? UpdateIp { get; set; }

        public string? UniqueKeyId { get; set; }
        public string? Statusname { get; set; }
        // ✅ Modify `Statusname` dynamically
        //public string Statusname => IsActive.HasValue && IsActive.Value ? "Active" : "Inactive";

    }
}
