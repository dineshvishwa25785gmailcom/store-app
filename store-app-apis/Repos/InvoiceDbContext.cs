using Microsoft.EntityFrameworkCore;
using store_app_apis.Modal.FolderInvoice;

namespace store_app_apis.Repos
{
    public class InvoiceDbContext : DbContext
    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
        {
        }

        //public DbSet<Company> Companies { get; set; }
        //public DbSet<Customer> Customers { get; set; }
       
        //public DbSet<SalesProductInfo> SalesProductInfos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            



        }

    }
}
