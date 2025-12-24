using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Repos.Models;
[Table("tbl_designation")]
public partial class TblDesignation
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreateIp { get; set; }

    public string? UpdateIp { get; set; }

    public bool? Isactive { get; set; }
    public bool? UniqueKeyID { get; set; }
}
