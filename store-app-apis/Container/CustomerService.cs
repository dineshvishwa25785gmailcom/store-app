using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using store_app_apis.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace store_app_apis.Container
{
    /// <summary>
    ///  This is Implimentation class of ICustomerService
    /// </summary>
    public class CustomerService : ICustomerService
    {



        ////await UniqueKeyGenerator.ResetKeyCounterAsync(context, "CUST"); // Resets Customer Key
        ////await UniqueKeyGenerator.ResetKeyCounterAsync(context, "EMP");  // Resets Employee Key


        private readonly StoreAppContext context;
        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;  // Inject Illoger for logging
        // Constractor 
        public CustomerService(StoreAppContext _context, IMapper mapper, ILogger<CustomerService> logger)
        {
            this.context = _context;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<APIResponse> Create(CustomerDTO data)
        {
            APIResponse response = new APIResponse();
            try
            {
                this.logger.LogInformation("Create Method Called");

                var newCustomer = new TblCustomer
                {
                    Name = data.Name,
                    Email = data.Email,
                    Phone = data.Phone,
                    IsActive = data.IsActive,
                    AddressDetails = data.AddressDetails,
                    UpdateDate = data.UpdateDate,
                    UpdateIp = data.UpdateIp,
                    CreateIp = data.CreateIp,
                    UniqueKeyID = await UniqueKeyGenerator.GenerateKeyAsync(context, "CUST"),
                    CountryCode = data.CountryCode,
                    CountryName = data.CountryName,
                    StateCode = data.StateCode,
                    StateName = data.StateName,
                    MobileNo = data.MobileNo,
                    AlternateMobile = data.AlternateMobile,
                    customer_company = data.customer_company,
                    gst_number = data.gst_number
                };

                await this.context.TblCustomers.AddAsync(newCustomer);  // ✅ Use correct entity name
                await this.context.SaveChangesAsync();

                response.responseCode = 201;
                response.Result = "pass";
            }
            catch (Exception ex)
            {
                response.responseCode = 400;
                response.ErrorMessage = ex.Message;
                this.logger.LogError(ex.Message, ex);
            }
            return response;
        }

        public async Task<List<CustomerDTO>> Getall()
        {
            try
            {
                var _data = await this.context.TblCustomers // ✅ Use correct entity name
                    .AsNoTracking()
                    .ToListAsync();

                var _response = _data.Select(c => new CustomerDTO
                {
                    Rec_Id = c.Rec_Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    IsActive = c.IsActive,
                    AddressDetails = c.AddressDetails,
                    CreateDate = c.CreateDate,
                    UpdateDate = c.UpdateDate,
                    CreateIp = c.CreateIp,
                    UpdateIp = c.UpdateIp,
                    UniqueKeyID = c.UniqueKeyID
                }).ToList();

                return _response; // ✅ Returning transformed DTO list
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching customers: {ex.Message}");
                return new List<CustomerDTO>(); // ✅ Returns empty list on failure
            }
        }
        public async Task<CustomerDTO> GetByUniqueKeyID(string UKID)
        {
            this.logger.LogInformation($"Fetching customer with UniqueKeyID: {UKID}");
            // ✅ Directly fetching CustomerDTONM without AutoMapper
            var entity = await this.context.TblCustomers.Where(c => c.UniqueKeyID == UKID).FirstOrDefaultAsync();
            var dto = this.mapper.Map<CustomerDTO>(entity);
            return dto;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                //TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                //await this.context.TblCustomers.AddAsync(_customer);
                //await this.context.SaveChangesAsync();
                var _customer = await this.context.TblCustomers.FirstOrDefaultAsync(c => c.UniqueKeyID == code);
                ;
                if (_customer != null)
                {
                    this.context.TblCustomers.Remove(_customer);
                    await this.context.SaveChangesAsync();

                    response.responseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.responseCode = 404;
                    response.ErrorMessage = "Data Not Found";
                }

                // return new APIResponse { Status = true, Message = "Record Created Successfully" };
            }
            catch (System.Exception ex)
            {
                //return new APIResponse { Status = false, Message = ex.Message };
                response.responseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public async Task<APIResponse> Update(CustomerDTO data, string UKID)
        {
            APIResponse response = new APIResponse();
            try
            {
                //TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                //await this.context.TblCustomers.AddAsync(_customer);
                //await this.context.SaveChangesAsync();
                //var _customer = await this.context.TblCustomers.FindAsync(UKID);
                var _customer = await this.context.TblCustomers.FirstOrDefaultAsync(c => c.UniqueKeyID == UKID);
                if (_customer != null)
                {
                    _customer.Name = data.Name;
                    _customer.Email = data.Email;
                    _customer.Phone = data.Phone;
                    _customer.IsActive = data.IsActive;
                    _customer.AddressDetails = data.AddressDetails;
                    _customer.CountryCode = data.CountryCode;
                    _customer.CountryName = data.CountryName;
                    _customer.StateCode = data.StateCode;
                    _customer.StateName = data.StateName;
                    _customer.MobileNo = data.MobileNo;
                    _customer.AlternateMobile = data.AlternateMobile;
                    _customer.customer_company = data.customer_company;
                    _customer.gst_number = data.gst_number;
                    _customer.CreateIp = data.CreateIp; // Assuming CreateIp is not updated
                    _customer.CreateDate = data.CreateDate; // Assuming CreateDate is not updated
                    // Update only the fields that are allowed to be updated
                    _customer.UpdateDate = data.UpdateDate ?? DateTime.Now; // Use provided date or current date
                    _customer.UpdateIp = data.UpdateIp;
                    await this.context.SaveChangesAsync();

                    response.responseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.responseCode = 404;
                    response.ErrorMessage = "Data Not Found";
                }

                // return new APIResponse { Status = true, Message = "Record Created Successfully" };
            }
            catch (System.Exception ex)
            {
                //return new APIResponse { Status = false, Message = ex.Message };
                response.responseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
