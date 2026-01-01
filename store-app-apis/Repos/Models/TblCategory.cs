using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace store_app_apis.Repos.Models;
public partial class TblCategory
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Rec_Id")]
    public int Rec_Id { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("UniqueKeyID")]
    public string? UniqueKeyID { get; set; } // ✅ Keep UniqueKeyID but not as a primary key

    [Column("Name")]
    public string? Name { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }

    [Column("create_ip")]
    public string? CreateIp { get; set; }

    [Column("update_ip")]
    public string? UpdateIp { get; set; }
    [Column("isactive")]
    public bool? IsActive { get; set; }
}
