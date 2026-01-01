using Microsoft.EntityFrameworkCore;
using store_app_apis.Models;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;

namespace store_app_apis.Container
{
    public class InvoiceRepository
    {
        private readonly StoreAppContext _context;

        public InvoiceRepository(StoreAppContext context)
        {
            _context = context;
        }

        public async Task<TblInvoiceHeader> GetInvoiceAsync(string invoiceYear, string invoiceNumber)
        {
            var invoice = await _context.TblInvoicesHeaders
                .AsNoTracking() // Prevent EF from tracking changes; speeds up performance
                .Include(i => i.Company) // Load company info
                .Include(i => i.Customer) // Load customer details from tbl_customer
                .Include(i => i.SalesItems) // Load invoice line items from tbl_sales_productinfo
                    .ThenInclude(si => si.Product) // 📦 Load product master details for each sales item
                .FirstOrDefaultAsync(i =>
                    i.InvoiceYear == invoiceYear &&
                    i.InvoiceNumber == invoiceNumber); // Filter by composite key

            return invoice; // Return fully hydrated entity
        }

        //public async Task<TblInvoiceHeader> GetInvoiceAsync(string invoiceYear, string invoiceNumber)
        //{
        //    // 🔍 Query the invoice header table using composite keys (invoiceYear + invoiceNumber)
        //    var invoice = await _context.TblInvoicesHeaders

        //        // ⚙️ Disable change tracking to improve performance and avoid EF projection issues
        //        .AsNoTracking()

        //        // 📎 Include related Company data from tbl_company
        //        .Include(i => i.Company)

        //        // 📎 Include related Customer data from tbl_customer
        //        .Include(i => i.Customer1)

        //        // 📎 Include invoice item details from tbl_sales_productinfo
        //        .Include(i => i.Items)

        //            // 📎 For each item, include nested Product info from Tbl_product
        //            .ThenInclude(item => item.Product)

        //        // 🔍 Filter by matching year and number to get a specific invoice
        //        .Where(i => i.InvoiceYear == invoiceYear && i.InvoiceNumber == invoiceNumber)

        //        // 🧩 Manually project into a new TblInvoiceHeader object
        //        .Select(i => new TblInvoiceHeader
        //        {
        //            // 🎯 Basic invoice metadata
        //            InvoiceYear = i.InvoiceYear,
        //            InvoiceNumber = i.InvoiceNumber,
        //            InvoiceDate = i.InvoiceDate,
        //            Destination = i.Destination,
        //            DispatchedThrough = i.DispatchedThrough,
        //            DeliveryNote = i.DeliveryNote,
        //            Remark = i.Remark,
        //            TotalAmount = i.TotalAmount,
        //            CgstRate = i.CgstRate,
        //            SgstRate = i.SgstRate,
        //            CgstAmount = i.CgstAmount,
        //            SgstAmount = i.SgstAmount,
        //            TotalGstAmount = i.TotalGstAmount,

        //            // 🏢 Company details mapped manually
        //            Company = new CompanyDTONM
        //            {
        //                CompanyId = i.Company.CompanyId,
        //                Name = i.Company.Name,
        //                AddressDetails = i.Company.AddressDetails,
        //                GstNumber = i.Company.GstNumber,
        //                CountryCode = i.Company.CountryCode,
        //                CountryName = i.Company.CountryName,
        //                StateCode = i.Company.StateCode,
        //                StateName = i.Company.StateName,
        //                AccountNumber = i.Company.AccountNumber,
        //                Ifsc = i.Company.Ifsc,
        //                AccountAddress = i.Company.AccountAddress,
        //                EmailId = i.Company.EmailId,
        //                MobileNo = i.Company.MobileNo,
        //                AlternateMobile = i.Company.AlternateMobile,
        //                Status = i.Company.Status
        //            },

        //            // 👤 Customer details mapped manually
        //            Customer1 = new TblCustomer
        //            {
        //                Rec_Id = i.Customer1.Rec_Id,
        //                UniqueKeyID = i.Customer1.UniqueKeyID,
        //                Name = i.Customer1.Name,
        //                Email = i.Customer1.Email,
        //                Phone = i.Customer1.Phone,
        //                IsActive = i.Customer1.IsActive,
        //                AddressDetails = i.Customer1.AddressDetails,
        //                CountryCode = i.Customer1.CountryCode,
        //                CountryName = i.Customer1.CountryName,
        //                StateCode = i.Customer1.StateCode,
        //                StateName = i.Customer1.StateName,
        //                MobileNo = i.Customer1.MobileNo,
        //                AlternateMobile = i.Customer1.AlternateMobile
        //            },

        //            // 📦 Item details including nested product information
        //            Items = i.Items.Select(item => new TblSalesProductinfo
        //            {
        //                InvoiceNumber = item.InvoiceNumber,
        //                ProductId = item.ProductId,
        //                Quantity = item.Quantity,
        //                RateWithoutTax = item.RateWithoutTax,
        //                RateWithTax = item.RateWithTax,
        //                Amount = item.Amount,

        //                // 📄 Nested product info for each item
        //                Product = new TblProduct
        //                {
        //                    RecId = item.Product.RecId,
        //                    ProductName = item.Product.ProductName,
        //                    Measurement = item.Product.Measurement,
        //                    HsnSacNumber = item.Product.HsnSacNumber,
        //                    CategoryCode = item.Product.CategoryCode,
        //                    TotalGstRate = item.Product.TotalGstRate,
        //                    CgstRate = item.Product.CgstRate,
        //                    ScgstRate = item.Product.ScgstRate,
        //                    RateWithoutTax = item.Product.RateWithoutTax,
        //                    RateWithTax = item.Product.RateWithTax,
        //                    CreateDate = item.Product.CreateDate,
        //                    UpdateDate = item.Product.UpdateDate,
        //                    CreateIp = item.Product.CreateIp,
        //                    UpdateIp = item.Product.UpdateIp,
        //                    Remark = item.Product.Remark,
        //                    IsActive = item.Product.IsActive,
        //                    UniqueKeyID = item.Product.UniqueKeyID
        //                }
        //            }).ToList()
        //        })

        //        // ✅ Materialize the result as a single invoice or return null if not found
        //        .FirstOrDefaultAsync();

        //    return invoice;
        //}

        //public async Task<TblInvoiceHeader> GetInvoiceAsync(string invoiceYear, string invoiceNumber)
        //{
        //    var invoice = await _context.TblInvoicesHeaders
        //        .Include(i => i.Company)
        //        .Include(i => i.Customer)
        //        .Include(i => i.Items)
        //        .Where(i => i.InvoiceYear == invoiceYear && i.InvoiceNumber == invoiceNumber)
        //        .Select(i => new TblInvoiceHeader
        //        {
        //            InvoiceYear = i.InvoiceYear,
        //            InvoiceNumber = i.InvoiceNumber,
        //            InvoiceDate = i.InvoiceDate,
        //            Destination = i.Destination,
        //            DispatchedThrough = i.DispatchedThrough,
        //            DeliveryNote = i.DeliveryNote,
        //            Remark = i.Remark,
        //            TotalAmount = i.TotalAmount,
        //            CgstRate = i.CgstRate,
        //            SgstRate = i.SgstRate,
        //            CgstAmount = i.CgstAmount,
        //            SgstAmount = i.SgstAmount,
        //            TotalGstAmount = i.TotalGstAmount,



        //            Company = new CompanyDTONM
        //            {
        //                CompanyId = i.Company.CompanyId,
        //                Name = i.Company.Name,
        //                AddressDetails = i.Company.AddressDetails,
        //                GstNumber = i.Company.GstNumber,
        //                CountryCode = i.Company.CountryCode,
        //                CountryName = i.Company.CountryName,
        //                StateCode = i.Company.StateCode,
        //                StateName = i.Company.StateName,
        //                AccountNumber = i.Company.AccountNumber,
        //                Ifsc = i.Company.Ifsc,
        //                AccountAddress = i.Company.AccountAddress,
        //                EmailId = i.Company.EmailId,
        //                MobileNo = i.Company.MobileNo,
        //                AlternateMobile = i.Company.AlternateMobile,
        //                Status = i.Company.Status
        //            },

        //            //Customer = new TblCustomer
        //            //{
        //            //    Rec_Id = i.Customer.Rec_Id,
        //            //    UniqueKeyID = i.Customer.UniqueKeyID,
        //            //    Name = i.Customer.Name,
        //            //    Email = i.Customer.Email,
        //            //    Phone = i.Customer.Phone,
        //            //    IsActive = i.Customer.IsActive,
        //            //    AddressDetails = i.Customer.AddressDetails,
        //            //    CountryCode = i.Customer.CountryCode,
        //            //    CountryName = i.Customer.CountryName,
        //            //    StateCode = i.Customer.StateCode,
        //            //    StateName = i.Customer.StateName,
        //            //    MobileNo = i.Customer.MobileNo,
        //            //    AlternateMobile = i.Customer.AlternateMobile
        //            //},



        //            Items = i.Items.Select(item => new TblSalesProductinfo
        //            {
        //                InvoiceNumber = item.InvoiceNumber,
        //                ProductId = item.ProductId,
        //                Quantity = item.Quantity,
        //                RateWithoutTax = item.RateWithoutTax,
        //                RateWithTax = item.RateWithTax,
        //                Amount = item.Amount,

        //                // ✅ Ensure Product details are included
        //                Product = new TblProduct
        //                {
        //                    RecId = item.Product.RecId,
        //                    ProductName = item.Product.ProductName,
        //                    Measurement = item.Product.Measurement,
        //                    HsnSacNumber = item.Product.HsnSacNumber,
        //                    CategoryCode = item.Product.CategoryCode,
        //                    TotalGstRate = item.Product.TotalGstRate,
        //                    CgstRate = item.Product.CgstRate,
        //                    ScgstRate = item.Product.ScgstRate,
        //                    RateWithoutTax = item.Product.RateWithoutTax,
        //                    RateWithTax = item.Product.RateWithTax,
        //                    CreateDate = item.Product.CreateDate,
        //                    UpdateDate = item.Product.UpdateDate,
        //                    CreateIp = item.Product.CreateIp,
        //                    UpdateIp = item.Product.UpdateIp,
        //                    Remark = item.Product.Remark,
        //                    IsActive = item.Product.IsActive,
        //                    UniqueKeyID = item.Product.UniqueKeyID
        //                }
        //            }).ToList()





        //            //Items = i.Items.Select(item => new SalesProductInfo
        //            //{

        //            //    InvoiceNumber = item.InvoiceNumber,
        //            //    ProductId = item.ProductId,
        //            //    ProductName = item.ProductName,
        //            //    Quantity = item.Quantity,
        //            //    RateWithoutTax = item.RateWithoutTax,
        //            //    RateWithTax = item.RateWithTax,
        //            //    Amount = item.Amount
        //            //}).ToList()
        //        })
        //        .FirstOrDefaultAsync();

        //    return invoice;
        //}
    }

}
