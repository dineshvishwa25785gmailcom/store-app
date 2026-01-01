using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace store_app_apis.Repos.Models
{
   
    public partial class TblMeasurement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Rec_Id")]
        public int Rec_Id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UniqueKeyID")]
       // [MaxLength(20)]
        public string UniqueKeyID { get; set; } = null!;

        [Column("Name")]
       // [MaxLength(500)]
        public string? Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }

        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        [Column("create_ip")]
       // [MaxLength(20)]
        public string? CreateIp { get; set; }

        [Column("update_ip")]
        //[MaxLength(20)]
        public string? UpdateIp { get; set; }

        [Column("isactive")]
        public bool? IsActive { get; set; }
    }
}
