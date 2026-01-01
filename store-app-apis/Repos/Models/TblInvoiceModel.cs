using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace store_app_apis.Repos.Models
{
    

    [Table("tbl_Invoice")] // Maps to your actual SQL table
    public class TblInvoiceModel
    {
        [Key]
        public string InvoiceNumber { get; set; }  // Assuming this is the PK

        [ForeignKey("InvoiceNumber")]
        public TblInvoiceHeader InvoiceHeader { get; set; }  // Linking to the header


        public DateTime InvoiceDate { get; set; }
        public int InvoiceYear { get; set; }
        public decimal TotalAmount { get; set; }
        public string Destination { get; set; }
        public string DispatchedThrough { get; set; }
        public string DeliveryNote { get; set; }
        public string Remark { get; set; }

        [ForeignKey("CompanyId")]
        public TblCompanyModel Company { get; set; }
        public int CompanyId { get; set; }

        [ForeignKey("CustomerId")]
        public TblCustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public List<TblProductItemModel> Products { get; set; }
    }

    public class TblCompanyModel
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class TblCustomerModel
    {
        [Key] // This tells EF that CustomerId is the primary key
        public int CustomerId { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }

    public class TblProductItemModel
    {
        [Key] // This marks ProductId as the primary key
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal RateWithoutTax { get; set; }
        public decimal RateWithTax { get; set; }
        public decimal Amount { get; set; }
    }
}
