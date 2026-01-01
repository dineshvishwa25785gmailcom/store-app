

using Microsoft.EntityFrameworkCore;
using store_app_apis.Container;
using store_app_apis.Modal;
using store_app_apis.Models;
using store_app_apis.Repos.Models;
using store_app_apis.Repos.Models.InvoiceListWithItems;
using store_app_apis.Service;
public interface IInvoiceContainer
{
    Task<List<Invoice_Header_DTO>> GetAllInvoiceHeaderByFilter(string year, DateTime? fromDate, DateTime? toDate, string dateType);
    Task<List<Invoice_Header_DTO>> GetAllInvoiceHeadersOnly();
    Task<Invoice_Header_DTO> GetAllInvoiceHeaderOnlybyCode(string invoiceno);
    Task<List<InvoiceFlatDto>> GetAllInvoiceCompCust();
    Task<List<SalesProductDTO>> GetAllSalesItemsByCode(string invoiceno);


    Task<ResponseType> Save(InvoiceCreateDTO invoiceEntity);
    Task<ResponseType> Remove(string invoiceno);

    
}
