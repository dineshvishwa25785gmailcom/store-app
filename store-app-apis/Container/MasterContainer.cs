 
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using store_app_apis.Repos.Models;
using store_app_apis.Repos;
using store_app_apis.Helper;
using DocumentFormat.OpenXml.InkML;
using store_app_apis.Modal;

namespace store_app_apis.Container
{
    public class MasterContainer : IMasterContainer

    {
        private readonly Repos.StoreAppContext  _DBContext;
        private readonly IMapper mapper;
        public MasterContainer(Repos.StoreAppContext dBContext, IMapper mapper)
        {

            this._DBContext = dBContext;
            this.mapper = mapper;
        }
 
         
        public async Task<APIResponse> SaveCategory(CategoryDTO category)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Result = string.Empty;
                int processcount = 0;
                if (category != null)
                {
                    using (var dbtransaction = await this._DBContext.Database.BeginTransactionAsync())
                    {
                        // check exist product
                        var _category = await this._DBContext.TblCategories.FirstOrDefaultAsync(item => item.UniqueKeyID == category.UniqueKeyId);
                        if (_category != null)
                        {
                            //update here
                            //_category.Id = category.Id;
                            _category.Name = category.Name;
                            _category.UpdateDate = DateTime.Now;
                            _category.UpdateIp = category.UpdateIp;
                            // await this._DBContext.SaveChangesAsync();
                        }
                        else
                        {
                            //var nextId = await this._DBContext.TblCategories.FromSqlRaw("SELECT NEXT VALUE FOR tbl_Category AS id").Select(p => p.Id).FirstOrDefaultAsync();
                            //// create new record
                            var _newcategory = new TblCategory()
                            {
                                UniqueKeyID = await UniqueKeyGenerator.GenerateKeyAsync(_DBContext, "CAT"),
                                Name = category.Name,
                                CreateIp = category.CreateIp,
                            };
                            await this._DBContext.TblCategories.AddAsync(_newcategory);
                            // await this._DBContext.SaveChangesAsync();
                        }
                        processcount++;
                        if (processcount > 0)
                        {
                            await this._DBContext.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                            response.Result = "pass";
                            response.ErrorMessage = "Data saved successfully";
                            response.responseCode = 200;
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                        }
                    }
                }
                else
                {
                    response.Result = "fail";
                    response.ErrorMessage = "Category Missing";
                    response.responseCode = 400;
                }
            }
            catch (Exception ex)
            {
                response.Result = "faile";
                response.ErrorMessage = "Failed to save";
                response.responseCode = 500;
            }
            return response;
        }
        public async Task<List<CategoryDTO>> CatGetall()
        {
            var productdata = await this._DBContext.TblCategories.ToListAsync();
            if (productdata != null && productdata.Count > 0)
            {
                // we need use automapper

                return this.mapper.Map<List<TblCategory>, List<CategoryDTO>>(productdata);
            }
            return new List<CategoryDTO>();

        }
        public async Task<CategoryDTO> CatGetbycode(string UKID)
        {
            var productdata = await this._DBContext.TblCategories.FirstOrDefaultAsync(item => item.UniqueKeyID.ToString() == UKID);
            if (productdata != null)
            {
                var _proddata = this.mapper.Map<TblCategory, CategoryDTO>(productdata);
                return _proddata;
            }
            return new CategoryDTO();
        }
        public async Task<List<CategoryDTO>> CatGetbycategory(string CategoryName)
        {
            var productdata = await this._DBContext.TblCategories.Where(item => item.Name == CategoryName).ToListAsync();
            if (productdata != null)
            {
                return this.mapper.Map<List<TblCategory>, List<CategoryDTO>>(productdata);
            }
            return new List<CategoryDTO>();
        }




        public async Task<APIResponse> Remove(string UKID)
        {
            APIResponse response = new APIResponse();
            try
            {
                //TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                //await this.context.TblCustomers.AddAsync(_customer);
                //await this.context.SaveChangesAsync();
                var _cat= await this._DBContext.TblCategories.FindAsync(UKID);
                if (_cat != null)
                {
                    this._DBContext.TblCategories.Remove(_cat);
                    await this._DBContext.SaveChangesAsync();

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


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<List<MeasurementDto>> MeasurmentGetall()
        {
            var productdata = await this._DBContext.TblMeasurements.Where(x =>x.IsActive==true).ToListAsync();
            if (productdata != null && productdata.Count > 0)
            {
                // we need use automapper

                return this.mapper.Map<List<TblMeasurement>, List<MeasurementDto>>(productdata);
            }
            return new List<MeasurementDto>();

        }






    }
}
