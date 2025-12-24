using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Repos.Models;
[Table("tbl_company")]
public partial class TblCompany
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string CompanyId { get; set; } = null!;

    public string? AddressDetails { get; set; }

    public string? AccountDetails { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreateIp { get; set; }

    public string? UpdateIp { get; set; }
}
