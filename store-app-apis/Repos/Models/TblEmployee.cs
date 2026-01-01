using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Repos.Models;
[Table("tbl_employee")]
public partial class TblEmployee
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Designation { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreateIp { get; set; }

    public string? UpdateIp { get; set; }

    public bool? Isactive { get; set; }
    public bool? UniqueKeyID { get; set; }
}
