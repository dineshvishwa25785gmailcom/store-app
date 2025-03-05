using AutoMapper;
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
        private readonly LearnDataContext context;
        private readonly IMapper mapper;

        // Constractor 
        public CustomerService(LearnDataContext _context, IMapper mapper)
        {
            this.context = _context;
            this.mapper = mapper;
        }

        public async Task<APIResponse> Create(Customermodal data)
        {

            APIResponse response = new APIResponse();   
            try
            {
                TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                await this.context.TblCustomers.AddAsync(_customer);
                await this.context.SaveChangesAsync();
                response.responseCode = 201;
                response.Result=data.Code;
                // return new APIResponse { Status = true, Message = "Record Created Successfully" };
            }
            catch (System.Exception ex)
            {
                //return new APIResponse { Status = false, Message = ex.Message };
                response.responseCode = 400;
                response.ErrorMessage=ex.Message;
            }   
            return response;
        }

        // This Synchrounuous Programming
        //public List<Customermodal> Getall()
        //{
        //    List<Customermodal> _response = new List<Customermodal>();  
        //  var _data= this.context.TblCustomers.ToList();
        //    if (_data != null)
        //    {
        //_response=this.mapper.Map<List<TblCustomer>, List<Customermodal>>(_data);
        //        foreach (var item in _data)
        //        {
        //            //Customermodal _temp = new Customermodal();
        //            //_temp.Code = item.Code;
        //            //_temp.Name = item.Name;
        //            //_temp.Email = item.Email;
        //            //_temp.Phone = item.Phone;
        //            //_temp.Creditlimit = item.Creditlimit;
        //            //_temp.IsActive = item.IsActive;
        //            //_temp.Taxcode = item.Taxcode;
        //            //_temp.Statusname = item.Statusname;
        //            //_response.Add(_temp);
        //        }
        //    }
        //    return _response;
        //    //throw new NotImplementedException();
        //}
        // This Asynchrounuous Programming  

        public async Task<List<Customermodal>> Getall()
        {
            List<Customermodal> _response = new List<Customermodal>();
            var _data = await this.context.TblCustomers.ToListAsync();
            if (_data != null)
            {
                _response = this.mapper.Map<List<TblCustomer>, List<Customermodal>>(_data);
            }
            return _response;
        }

        public async  Task<Customermodal> Getbycode(string code)
        {
            Customermodal  _response = new  Customermodal() ;
            var _data = await this.context.TblCustomers.FindAsync(code);
            if (_data != null)
            {
                _response = this.mapper.Map<TblCustomer, Customermodal>(_data);
            }
            return _response;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                //TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                //await this.context.TblCustomers.AddAsync(_customer);
                //await this.context.SaveChangesAsync();
                 var _customer =await this.context.TblCustomers.FindAsync(code);
                if (_customer != null)
                {
                    this.context.TblCustomers.Remove(_customer);
                    await this.context.SaveChangesAsync();

                    response.responseCode = 201;
                    response.Result = code;
                }
                else  {
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

        public async Task<APIResponse> Update(Customermodal data, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                //TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                //await this.context.TblCustomers.AddAsync(_customer);
                //await this.context.SaveChangesAsync();
                var _customer = await this.context.TblCustomers.FindAsync(code);
                if (_customer != null)
                {
                    _customer.Name = data.Name; 
                    _customer.Email = data.Email;       
                    _customer.Phone = data.Phone;
                    _customer.Creditlimit = data.Creditlimit;
                    _customer.IsActive = data.IsActive; 
                    await this.context.SaveChangesAsync();

                    response.responseCode = 201;
                    response.Result = code;
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
