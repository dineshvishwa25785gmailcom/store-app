using AutoMapper;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Models;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using Microsoft.AspNetCore.Mvc;
using store_app_apis.Repos.Models.InvoiceListWithItems;
using store_app_apis.Modal; // ✅ Ensure this is included


namespace store_app_apis.Container
{
    //https://www.youtube.com/watch?v=t_dSvSZ0f1A

    public class InvoiceContainer : IInvoiceContainer
    {
        private readonly Repos.StoreAppContext _DBContext;
        private readonly IMapper mapper;
        private readonly ILogger<InvoiceContainer> _logger;
        public InvoiceContainer(Repos.StoreAppContext dBContext, IMapper mapper, ILogger<InvoiceContainer> _logger)
        {
            this._DBContext = dBContext;
            this.mapper = mapper;
            this._logger = _logger;
        }

        //public async Task<List<Invoice_Header_DTO>> GetAllInvoiceHeadersOnly()
        //{
        //    //try
        //    //{
        //    //    this._logger.LogInformation("Fetching all invoices from the database...");

        //    //    var _data = await this._DBContext.TblInvoicesHeaders.ToListAsync(); // ✅ Gets all records

        //    //    return (_data != null && _data.Count > 0)
        //    //        ? this.mapper.Map<List<TblInvoiceHeader>, List<Invoice_Header_DTO>>(_data)
        //    //        : new List<Invoice_Header_DTO>();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    this._logger.LogError($"Error in GetAllInvoiceHeadersOnly: {ex.Message}", ex);
        //    //    throw;
        //    //}




        //}

        public async Task<List<Invoice_Header_DTO>> GetAllInvoiceHeadersOnly()
        {
            try
            {
                this._logger.LogInformation("Fetching all invoices from the database...");

                var _data = await (
    from invoice in this._DBContext.TblInvoicesHeaders
    join company in this._DBContext.TblCompanies
        on invoice.CompanyId equals company.CompanyId into compGroup
    from company in compGroup.DefaultIfEmpty()
    join customer in this._DBContext.TblCustomers
        on invoice.CustomerId equals customer.UniqueKeyID into custGroup
    from customer in custGroup.DefaultIfEmpty()
    select new Invoice_Header_DTO
    {
        InvoiceYear = invoice.InvoiceYear,
        InvoiceNumber = invoice.InvoiceNumber,
        InvoiceDate = invoice.InvoiceDate,
        Totalamount = invoice.TotalAmount,
        CompanyName = company != null ? company.Name : null,
        CustomerName = customer != null ? customer.Name : null
        // Add other fields as needed
    }).ToListAsync();

                return _data ?? new List<Invoice_Header_DTO>();
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error in GetAllInvoiceHeadersOnly: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<InvoiceFlatDto>> GetAllInvoiceCompCust()
        {
            var result = await (
                from a in _DBContext.TblInvoicesHeaders
                join b in _DBContext.TblCompanies on a.CompanyId equals b.CompanyId
                join c in _DBContext.TblCustomers on a.CustomerId equals c.UniqueKeyID
                select new InvoiceFlatDto
                {
                    // Company
                    CoName = b.Name,
                    CoAddr = b.AddressDetails,
                    CoGST = b.GstNumber,
                    CntryCode = b.CountryCode,
                    CntryName = b.CountryName,
                    StCode = b.StateCode,
                    StName = b.StateName,
                    AccNum = b.AccountNumber,
                    IFSC = b.Ifsc,
                    AccAddr = b.AccountAddress,
                    CoEmail = b.EmailId,
                    CoMob = b.MobileNo,
                    CoAltMob = b.AlternateMobile,
                    CoStatus = b.Status,

                    // Customer
                    CuName = c.Name,
                    CuEmail = c.Email,
                    CuPhone = c.Phone,
                    CuStatus = c.IsActive,
                    CuAddr = c.AddressDetails,
                    CuCntryCode = c.CountryCode,
                    CuCntryName = c.CountryName,
                    CuStCode = c.StateCode,
                    CuStName = c.StateName,
                    CuMob = c.MobileNo,
                    CuAltMob = c.AlternateMobile,

                    // Invoice
                    InvYear = a.InvoiceYear,
                    InvNum = a.InvoiceNumber,
                    InvDate = a.InvoiceDate,
                    CoID = a.CompanyId,
                    CuID = a.CustomerId,
                    Dest = a.Destination,
                    DispThrough = a.DispatchedThrough,
                    DelNote = a.DeliveryNote,
                    Remark = a.Remark,
                    TotalAmt = a.TotalAmount
                }).ToListAsync();

            return result;
        }

        //public async Task<List<InvoiceHeaderWithItemsDto>> GetAllInvoiceHeadersWithDetails()
        //{
        //    var query = from invoice in _DBContext.TblInvoicesHeaders
        //                join customer in _DBContext.TblCustomers
        //                    on invoice.CustomerId equals customer.UniqueKeyID
        //                join company in _DBContext.TblCompanies
        //                    on invoice.CompanyId equals company.CompanyId
        //                select new InvoiceHeaderWithItemsDto
        //                {
        //                    InvoiceNumber = invoice.InvoiceNumber,
        //                    InvoiceYear = invoice.InvoiceYear,
        //                    InvoiceDate = invoice.InvoiceDate,
        //                    TotalAmount = invoice.TotalAmount,
        //                    CgstRate = invoice.CgstRate,
        //                    SgstRate = invoice.SgstRate,
        //                    CgstAmount = invoice.CgstAmount,
        //                    SgstAmount = invoice.SgstAmount,
        //                    TotalGstAmount = invoice.TotalGstAmount,
        //                    GrandTotalAmount = invoice.grand_total_amount,
        //                    Destination = invoice.Destination,
        //                    DeliveryNote = invoice.DeliveryNote,
        //                    DispatchedThrough = invoice.DispatchedThrough,
        //                    Remark = invoice.Remark,

        //                    Company = new CompanyDto
        //                    {
        //                        CompanyId = company.CompanyId,
        //                        Name = company.Name,
        //                        GstNumber = company.GstNumber,
        //                        EmailId = company.EmailId,
        //                        MobileNo = company.MobileNo
        //                    },
        //                    Customer = new CustomerDto
        //                    {
        //                        CustomerId = customer.UniqueKeyID,
        //                        Name = customer.Name,
        //                        Phone = customer.Phone,
        //                        Email = customer.Email
        //                    },
        //                    Items = _DBContext.TblSalesProductinfo_S
        //                        .Where(item => item.InvoiceNumber == invoice.InvoiceNumber)
        //                        .Select(item => new ProductItemDto
        //                        {
        //                            ProductId = item.ProductId,
        //                            //ProductName = item.,
        //                            Quantity = item.Quantity,
        //                            RateWithoutTax = item.RateWithoutTax,
        //                            RateWithTax = item.RateWithTax,
        //                            Amount = item.Amount
        //                        }).ToList()
        //                };

        //    return await query.ToListAsync();
        //}



        public async Task<List<Invoice_Header_DTO>> GetAllInvoiceHeaderByFilter(string? year, DateTime? fromDate, DateTime? toDate, string? dateType)
        {
            try
            {
                this._logger.LogInformation($"Fetching Invoice Headers - Year: {year ?? "NULL"}, From: {fromDate}, To: {toDate}, Date Type: {dateType}");

                var query = this._DBContext.TblInvoicesHeaders.AsQueryable();

                // ✅ Apply Year filter if provided
                if (!string.IsNullOrEmpty(year))
                {
                    query = query.Where(i => i.InvoiceYear == year);
                }

                // ✅ Apply Date Range filter based on selected date type
                if (fromDate.HasValue && toDate.HasValue)
                {
                    if (dateType?.ToLower() == "i")
                    {
                        query = query.Where(i => i.InvoiceDate >= fromDate.Value && i.InvoiceDate <= toDate.Value);
                    }
                    else if (dateType?.ToLower() == "c")
                    {
                        query = query.Where(i => i.CreateDate >= fromDate.Value && i.CreateDate <= toDate.Value);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid dateType. Use 'invoice' or 'create'.");
                    }
                }

                var _data = await query.ToListAsync();

                return (_data != null && _data.Count > 0)
                    ? this.mapper.Map<List<TblInvoiceHeader>, List<Invoice_Header_DTO>>(_data)
                    : new List<Invoice_Header_DTO>();
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error in GetAllInvoiceHeader: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<Invoice_Header_DTO> GetAllInvoiceHeaderOnlybyCode(string invoiceno)
        {
            try
            {
                this._logger.LogInformation("GetAllInvoiceHeaderbyCode");
                var _data = await this._DBContext.TblInvoicesHeaders.FirstOrDefaultAsync(item => item.InvoiceNumber == invoiceno);
                if (_data != null)
                {
                    return this.mapper.Map<TblInvoiceHeader, Invoice_Header_DTO>(_data);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in GetAllInvoiceHeaderbyCode:" + ex.Message.ToString());
            }

            return new Invoice_Header_DTO();
        }

        public async Task<List<SalesProductDTO>> GetAllSalesItemsByCode(string invoiceno)
        {
            try
            {
                _logger.LogInformation("GetAllSalesItemsByCode");

                var joinedData = await (from sp in _DBContext.TblSalesProductinfo_S
                                        join p in _DBContext.TblProducts
                                        on sp.ProductId equals p.UniqueKeyID
                                        where sp.InvoiceNumber == invoiceno
                                        select new SalesProductDTO
                                        {
                                            InvoiceNumber = sp.InvoiceNumber,
                                            ProductId = sp.ProductId,
                                            ProductName = p.ProductName,
                                            Quantity = sp.Quantity,

                                            Amount = sp.Amount,
                                            CreateDate = sp.CreateDate,
                                            Measurement = p.Measurement,
                                            HsnSacNumber = p.HsnSacNumber,
                                            TotalGstRate = p.TotalGstRate,
                                            CgstRate = p.CgstRate,
                                            ScgstRate = p.ScgstRate,
                                            CategoryCode = p.CategoryCode
                                        }).ToListAsync();

                if (joinedData != null && joinedData.Count > 0)
                {
                    return joinedData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllSalesItemsByCode: " + ex.Message);
            }

            return new List<SalesProductDTO>();
        }
        public async Task<ResponseType> Save(InvoiceCreateDTO invoiceEntity)
        {
            var response = new ResponseType();
            int processCount = 0;

            try
            {
                _logger.LogInformation("Starting invoice save process...");

                if (invoiceEntity == null)
                {
                    response.Result = "fail";
                    return response;
                }

                using var dbTransaction = await _DBContext.Database.BeginTransactionAsync();

                string invoiceKey = await SaveHeader(invoiceEntity);

                if (!string.IsNullOrEmpty(invoiceKey) &&
                    invoiceEntity.Products != null &&
                    invoiceEntity.Products.Count > 0)
                {
                    foreach (var item in invoiceEntity.Products)
                    {
                        item.InvoiceNumber = invoiceEntity.InvoiceNumber; // Ensure linkage
                        item.InvoiceYear = invoiceEntity.InvoiceYear;     // ✅ ADD THIS LINE

                        bool saveResult = await SaveDetail(item, invoiceEntity.CreateBy);

                        if (saveResult)
                            processCount++;
                    }

                    if (processCount == invoiceEntity.Products.Count)
                    {
                        await _DBContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();

                        response.Result = "pass";
                        response.KyValue = invoiceKey;
                    }
                    else
                    {
                        await dbTransaction.RollbackAsync();
                        response.Result = "fail";
                    }
                }
                else
                {
                    await dbTransaction.RollbackAsync();
                    response.Result = "fail";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Save Invoice: {ex.Message}");
                response.Result = "fail";
            }

            return response;
        }

        private async Task<string> SaveHeader(InvoiceCreateDTO invoiceHeader)
        {
            try
            {
                var existingHeader = await _DBContext.TblInvoicesHeaders
                    .FirstOrDefaultAsync(item => item.InvoiceNumber == invoiceHeader.InvoiceNumber);

                if (existingHeader != null)
                {
                    existingHeader.CustomerId = invoiceHeader.CustomerId;
                    existingHeader.CompanyId = invoiceHeader.CompanyId;
                    existingHeader.Destination = invoiceHeader.Destination;
                    existingHeader.DispatchedThrough = invoiceHeader.DispatchedThrough;
                    existingHeader.DeliveryNote = invoiceHeader.DeliveryNote;
                    existingHeader.Remark = invoiceHeader.Remark;
                    existingHeader.UpdateBy = invoiceHeader.UpdateBy;
                    existingHeader.UpdateDate = DateTime.Now;
                    existingHeader.UpdateIp = invoiceHeader.UpdateIp;
                    existingHeader.TotalAmount = invoiceHeader.TotalAmount;

                    var existingDetails = await _DBContext.TblSalesProductinfo_S
                        .Where(item => item.InvoiceNumber == invoiceHeader.InvoiceNumber)
                        .ToListAsync();

                    if (existingDetails.Any())
                    {
                        _DBContext.TblSalesProductinfo_S.RemoveRange(existingDetails);
                    }
                }
                else
                {
                    var newHeader = mapper.Map<InvoiceCreateDTO, TblInvoiceHeader>(invoiceHeader);
                    await _DBContext.TblInvoicesHeaders.AddAsync(newHeader);
                }

                return invoiceHeader.InvoiceNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SaveHeader Error: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task<bool> SaveDetail(InvoiceItemCreateDTO invoiceDetail, string createdBy)
        {
            try
            {
                var detail = mapper.Map<InvoiceItemCreateDTO, TblSalesProductinfo>(invoiceDetail);
                detail.CreateBy = createdBy;
                detail.CreateDate = DateTime.Now;

                await _DBContext.TblSalesProductinfo_S.AddAsync(detail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SaveDetail Error: {ex.Message}");
                return false;
            }
        }

        public async Task<ResponseType> Remove(string invoiceno)
        {
            try
            {
                using (var dbtransaction = await this._DBContext.Database.BeginTransactionAsync())
                {
                    var _data = await this._DBContext.TblInvoicesHeaders.FirstOrDefaultAsync(item => item.InvoiceNumber == invoiceno);
                    if (_data != null)
                    {
                        this._DBContext.TblInvoicesHeaders.Remove(_data);
                    }

                    var _detdata = await this._DBContext.TblSalesProductinfo_S.Where(item => item.InvoiceNumber == invoiceno).ToListAsync();
                    if (_detdata != null && _detdata.Count > 0)
                    {
                        this._DBContext.TblSalesProductinfo_S.RemoveRange(_detdata);
                    }
                    await this._DBContext.SaveChangesAsync();
                    await dbtransaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {

                this._logger.LogError(ex, "Remove:" + ex.Message.ToString());
            }
            return new ResponseType() { Result = "pass", KyValue = invoiceno };
        }









        //public static Invoice GetInvoiceDetails(string invoiceNumber)
        //{
        //    // Simulated database fetch (replace with actual DB queries)
        //    var company = new Company
        //    {
        //        CompanyId = 1,
        //        Name = "ABC Pvt Ltd",
        //        Address = "XYZ Street, City",
        //        GSTNumber = "GST12345",
        //        Country = "India",
        //        State = "Uttar Pradesh",
        //        AccountNumber = "123456789",
        //        IFSC = "IFSC0001",
        //        Email = "contact@abc.com",
        //        Mobile = "9876543210"
        //    };

        //    var customer = new Customer
        //    {
        //        CustomerId = 101,
        //        Name = "John Doe",
        //        Address = "123 Main St",
        //        Email = "john@example.com",
        //        Phone = "9998887776",
        //        IsActive = true
        //    };

        //    var products = new List<ProductItem>
        //{
        //    new ProductItem { ProductId = 1, ProductName = "Laptop", Quantity = 1, RateWithoutTax = 50000, RateWithTax = 54000, Amount = 54000 },
        //    new ProductItem { ProductId = 2, ProductName = "Mouse", Quantity = 2, RateWithoutTax = 500, RateWithTax = 550, Amount = 1100 }
        //};

        //    return new Invoice
        //    {
        //        InvoiceNumber = invoiceNumber,
        //        InvoiceYear = DateTime.Now.Year,
        //        InvoiceDate = DateTime.Now,
        //        Company = company,
        //        Customer = customer,
        //        Destination = "New Delhi",
        //        DispatchedThrough = "Courier",
        //        DeliveryNote = "Handle with care",
        //        Remark = "Urgent delivery",
        //        TotalAmount = 55100,
        //        Products = products
        //    };
        //}










    }
}
