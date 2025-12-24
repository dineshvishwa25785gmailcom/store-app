using Microsoft.EntityFrameworkCore;
using store_app_apis.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace store_app_apis.Repos.Models;
public partial class TblProduct
{
    [Key]
    [Column("UniqueKeyID")]
    public string UniqueKeyID { get; set; }  // Primary Key

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Rec_Id")]
    public int RecId { get; set; }  // Auto-incremented Identity Column

    [Column("product_name")]
    public string? ProductName { get; set; }

    [Column("measurement")]
    public string? Measurement { get; set; }

    [Column("hsn_sac_number")]
    public string? HsnSacNumber { get; set; }

    [Column("category_code")]
    public string? CategoryCode { get; set; }

    [Column("total_gst_rate")]
    public decimal? TotalGstRate { get; set; }

    [Column("cgst_rate")]
    public decimal? CgstRate { get; set; }

    [Column("scgst_rate")]
    public decimal? ScgstRate { get; set; }

    [Column("rate_without_tax")]
    public decimal? RateWithoutTax { get; set; }

    [Column("rate_with_tax")]
    public decimal? RateWithTax { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }

    [Column("create_ip")]
    public string? CreateIp { get; set; }

    [Column("update_ip")]
    public string? UpdateIp { get; set; }

    [Column("remark")]
    public string? Remark { get; set; }

    [Column("isactive")]
    public bool? IsActive { get; set; }

}

