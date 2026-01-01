using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Modal;
using store_app_apis.Models;
using store_app_apis.Repos.Models;
namespace store_app_apis.Repos;

public partial class StoreAppContext : DbContext
{

    public StoreAppContext()
    {
    }
    public StoreAppContext(DbContextOptions<StoreAppContext> options)
        : base(options)
    {
    }

    public DbSet<UniqueKeyTracker> UniqueKeyTrackers { get; set; } // ✅ Register the entity
    public virtual DbSet<CompanyDTONM> TblCompanies { get; set; }
    public virtual DbSet<TblCustomer> TblCustomers { get; set; }
    public virtual DbSet<TblMenu> TblMenus { get; set; }
    public virtual DbSet<TblOtpManager> TblOtpManagers { get; set; }
    public virtual DbSet<TblProductimage> TblProductimages { get; set; }
    public virtual DbSet<TblPwdManger> TblPwdMangers { get; set; }
    public virtual DbSet<TblRefreshtoken> TblRefreshtokens { get; set; }
    public virtual DbSet<TblRole> TblRoles { get; set; }
    public virtual DbSet<TblRolepermission> TblRolepermissions { get; set; }
    public virtual DbSet<TblTempuser> TblTempusers { get; set; }
    public virtual DbSet<TblUser> TblUsers { get; set; }

    /// <summary>
    /// </summary>
    public virtual DbSet<TblCategory> TblCategories { get; set; } = null!;
    public virtual DbSet<TblMeasurement> TblMeasurements { get; set; } = null!;


    //public virtual DbSet<TblSalesHeader> TblSalesHeaders { get; set; } = null!;
    //public virtual DbSet<TblSalesProductInfo> TblSalesProductInfos { get; set; } = null!;

    public virtual DbSet<TblDesignation> TblDesignations { get; set; }
    public virtual DbSet<TblEmployee> TblEmployees { get; set; }
    public virtual DbSet<TblInvoiceHeader> TblInvoicesHeaders { get; set; }
    public virtual DbSet<TblSalesProductinfo> TblSalesProductinfo_S { get; set; }
    // public virtual DbSet<SalesProductInfoDTONM> SalesProductInfoDTONM_S { get; set; }








    public virtual DbSet<TblProduct> TblProducts { get; set; } = null!;




    /// <param name="modelBuilder"></param>
    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=tcp:SANJU\\MSSQLSERVER_2022;Initial Catalog=test_db;Persist Security Info=True;User ID=sa;Password=Di@251521;TrustServerCertificate=True;");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UniqueKeyTracker>().ToTable("UniqueKeyTracker"); // ✅ Map to table
        modelBuilder.Entity<UniqueKeyTracker>().HasKey(t => t.Prefix); // ✅ Define primary key


        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.UniqueKeyID).HasName("PK_Tbl_product");

            entity.Property(e => e.RecId)
                  .UseIdentityColumn(1001, 1); // Matches IDENTITY(1001,1) in SQL

            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Measurement).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.HsnSacNumber).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.CategoryCode).HasMaxLength(20).IsUnicode(false);

            entity.Property(e => e.TotalGstRate).HasColumnType("numeric(18, 3)");
            entity.Property(e => e.CgstRate).HasColumnType("numeric(18, 3)");
            entity.Property(e => e.ScgstRate).HasColumnType("numeric(18, 3)");
            entity.Property(e => e.RateWithoutTax).HasColumnType("numeric(18, 3)");
            entity.Property(e => e.RateWithTax).HasColumnType("numeric(18, 3)");

            entity.Property(e => e.CreateDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");

            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.CreateIp).HasMaxLength(20);
            entity.Property(e => e.UpdateIp).HasMaxLength(20);
            entity.Property(e => e.Remark).HasMaxLength(200);
            entity.Property(e => e.IsActive);

            entity.ToTable("Tbl_product");
        });







        modelBuilder.Entity<TblCustomer>().ToTable("tbl_customer");
        modelBuilder.Entity<TblCustomer>().HasKey(c => c.UniqueKeyID);
        modelBuilder.Entity<TblCustomer>().HasIndex(c => c.UniqueKeyID).IsUnique();
        modelBuilder.Entity<TblCustomer>().Property(e => e.UniqueKeyID).ValueGeneratedOnAdd();
        modelBuilder.Entity<TblCustomer>().Property(e => e.Rec_Id).ValueGeneratedOnAdd();
        // NOTE: CustomerDTO is a DTO, not a database entity - do not configure it as an entity
        // Use TblCustomer instead for any customer data access




        modelBuilder.Entity<CompanyDTONM>().ToTable("tbl_company");



        modelBuilder.Entity<CompanyDTONM>().HasKey(c => c.CompanyId);


        modelBuilder.Entity<CompanyDTONM>()
            .Property(c => c.CompanyId)
            .HasColumnType("nvarchar(20)");




        modelBuilder.Entity<TblInvoiceHeader>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceYear, e.InvoiceNumber }).HasName("PK_Tbl_Invoice");

            entity.ToTable("tbl_Invoice");

            entity.Property(e => e.InvoiceYear)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("invoice_year");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(20)
                .HasColumnName("invoice_number");
            entity.Property(e => e.CompanyId)
                .HasMaxLength(10)
                .HasColumnName("company_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateIp)
                .HasMaxLength(20)
                .HasColumnName("create_ip");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("customer_id");
            entity.Property(e => e.DeliveryNote)
                .HasMaxLength(200)
                .HasColumnName("delivery_note");
            entity.Property(e => e.Destination)
                .HasMaxLength(200)
                .HasColumnName("destination");
            entity.Property(e => e.DispatchedThrough)
                .HasMaxLength(200)
                .HasColumnName("dispatched_through");
            entity.Property(e => e.InvoiceDate)
                .HasColumnType("datetime")
                .HasColumnName("invoice_date");
            entity.Property(e => e.RecId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Rec_Id");
            entity.Property(e => e.Remark)
                .HasMaxLength(20)
                .HasColumnName("remark");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("totalamount");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateIp)
                .HasMaxLength(20)
                .HasColumnName("update_ip");
        });
        modelBuilder.HasSequence("Rec_ID").StartsAt(1000L);
        /// aaded 25/10/2025
        modelBuilder.Entity<TblSalesProductinfo>(entity =>
        {
            entity.ToTable("tbl_sales_productinfo");

            entity.HasKey(e => new { e.InvoiceYear, e.InvoiceNumber, e.ProductId })
                  .HasName("PK_tbl_SalesInvoiceDetail");

            entity.Property(e => e.RecId).HasColumnName("Rec_ID").ValueGeneratedOnAdd();

            entity.HasOne(sp => sp.InvoiceHeader)
                  .WithMany(h => h.SalesItems)
                  .HasForeignKey(sp => new { sp.InvoiceYear, sp.InvoiceNumber })
                  .HasPrincipalKey(h => new { h.InvoiceYear, h.InvoiceNumber });
        });

       
        modelBuilder.Entity<TblCategory>().ToTable("tbl_Category");
        modelBuilder.Entity<TblCategory>().HasKey(c => c.UniqueKeyID);
        modelBuilder.Entity<TblCategory>().HasIndex(c => c.UniqueKeyID).IsUnique();
        modelBuilder.Entity<TblCategory>().Property(e => e.UniqueKeyID).ValueGeneratedOnAdd();
        modelBuilder.Entity<TblCategory>().Property(e => e.Rec_Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<TblMeasurement>().ToTable("tbl_Measurement");
        modelBuilder.Entity<TblMeasurement>().HasKey(c => c.UniqueKeyID);
        modelBuilder.Entity<TblMeasurement>().HasIndex(c => c.UniqueKeyID).IsUnique();
        modelBuilder.Entity<TblMeasurement>().Property(e => e.UniqueKeyID).ValueGeneratedOnAdd();
        modelBuilder.Entity<TblMeasurement>().Property(e => e.Rec_Id).ValueGeneratedOnAdd();
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
