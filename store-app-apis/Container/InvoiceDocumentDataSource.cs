using Microsoft.EntityFrameworkCore;
using store_app_apis.Repos.Models;
using store_app_apis.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InvoiceDocumentDataSource
{
    private readonly StoreAppContext _DBContext;

    public InvoiceDocumentDataSource(StoreAppContext dBContext)
    {
        _DBContext = dBContext;
    }

    public async Task<InvoiceModel> GetInvoiceDetails(string invoiceNumber)
    {




        var invoiceData = await _DBContext.TblInvoiceModels2
            .Where(inv => inv.InvoiceNumber == invoiceNumber)
            .Include(inv => inv.Company)
            .Include(inv => inv.Customer)
            .Include(inv => inv.Products)
            .FirstOrDefaultAsync();

        if (invoiceData == null) return null;

        return new InvoiceModel
        {
            InvoiceNumber = invoiceData.InvoiceNumber,
            InvoiceDate = invoiceData.InvoiceDate,
            InvoiceYear = invoiceData.InvoiceYear,
            TotalAmount = invoiceData.TotalAmount,
            Destination = invoiceData.Destination,
            DispatchedThrough = invoiceData.DispatchedThrough,
            DeliveryNote = invoiceData.DeliveryNote,
            Remark = invoiceData.Remark,
            Company = new CompanyModel
            {
                CompanyId = invoiceData.Company.CompanyId,
                Name = invoiceData.Company.Name,
                Address = invoiceData.Company.Address,
                GSTNumber = invoiceData.Company.GSTNumber,
                Country = invoiceData.Company.Country,
                State = invoiceData.Company.State,
                AccountNumber = invoiceData.Company.AccountNumber,
                IFSC = invoiceData.Company.IFSC,
                Email = invoiceData.Company.Email,
                Mobile = invoiceData.Company.Mobile
            },
            Customer = new CustomerModel
            {
                CustomerId = invoiceData.Customer.CustomerId,
                Name = invoiceData.Customer.Name,
                Address = invoiceData.Customer.Address,
                Email = invoiceData.Customer.Email,
                Phone = invoiceData.Customer.Phone,
                IsActive = invoiceData.Customer.IsActive
            },
            Products = invoiceData.Products.Select(p => new ProductItemModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Quantity = p.Quantity,
                RateWithoutTax = p.RateWithoutTax,
                RateWithTax = p.RateWithTax,
                Amount = p.Amount
            }).ToList()
        };
    }
}