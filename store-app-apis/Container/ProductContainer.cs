using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;

namespace store_app_apis.Container
{
    public class ProductContainer : IProductService
    {
        private readonly Repos.StoreAppContext _DBContext;
        private readonly IMapper mapper;
        public ProductContainer(Repos.StoreAppContext dBContext, IMapper mapper)
        {

            this._DBContext = dBContext;
            this.mapper = mapper;
        }
        public async Task<List<ProductDTO>> Getall()
        {
            var productlist = await this._DBContext.TblProducts.ToListAsync();
            if (productlist != null && productlist.Count > 0)
            {
                // Correctly map the list
                var _proddata = this.mapper.Map<List<TblProduct>, List<ProductDTO>>(productlist);
                return _proddata;
            }
            else
            {
                return new List<ProductDTO>();
            }
        }

        public async Task<ProductDTO> Getbycode(string code)
        {
            var productdata = await this._DBContext.TblProducts.FirstOrDefaultAsync(item => item.UniqueKeyID == code);
            if (productdata != null)
            {
                var _proddata = this.mapper.Map<TblProduct, ProductDTO>(productdata);
                return _proddata;
            }
            return new ProductDTO();
        }
        public async Task<List<ProductDTO>> Getbyname(string name)
        {
            var productdata = await this._DBContext.TblProducts.Where(item => item.ProductName == name).ToListAsync();
            if (productdata != null)
            {
                return this.mapper.Map<List<TblProduct>, List<ProductDTO>>(productdata);
            }
            return new List<ProductDTO>();
        }
        public async Task<APIResponse> SaveProduct(ProductDTO product)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Result = string.Empty;
                int processcount = 0;
                if (product != null)
                {
                    using (var dbtransaction = await this._DBContext.Database.BeginTransactionAsync())
                    {
                        // check exist product
                        var _product = await this._DBContext.TblProducts.FirstOrDefaultAsync(item => item.UniqueKeyID.ToString() == product.UniqueKeyID.ToString());
                        if (_product != null)
                        {
                            //update here
                            _product.ProductName = product.ProductName;
                            _product.Measurement = product.Measurement;
                            _product.HsnSacNumber = product.HsnSacNumber;
                            _product.Remark = product.Remark;
                            _product.CategoryCode = product.CategoryCode;
                            _product.TotalGstRate = product.TotalGstRate;
                            _product.CgstRate = product.CgstRate;
                            _product.ScgstRate = product.ScgstRate;
                            _product.RateWithTax = product.RateWithTax;
                            _product.RateWithoutTax = product.RateWithoutTax;
                            _product.UpdateDate = DateTime.Now;
                            _product.UpdateIp = product.UpdateIp;
                            _product.IsActive = product.IsActive;
                            _product.Remark = product.Remark;
                            
                            // await this._DBContext.SaveChangesAsync();
                        }
                        else
                        {
                            // create new record
                            var _newproduct = new TblProduct()
                            {
                                ProductName = product.ProductName,
                                Measurement = product.Measurement,
                                HsnSacNumber = product.HsnSacNumber,
                                CategoryCode = product.CategoryCode,
                                TotalGstRate = product.TotalGstRate,
                                CgstRate = product.CgstRate,
                                ScgstRate = product.ScgstRate,
                                RateWithTax = product.RateWithTax,
                                RateWithoutTax = product.RateWithoutTax,
                                CreateIp = product.CreateIp,
                                IsActive = product.IsActive,
                                Remark = product.Remark,
                                UniqueKeyID = await UniqueKeyGenerator.GenerateKeyAsync(_DBContext, "PROD"),
                            };
                            await this._DBContext.TblProducts.AddAsync(_newproduct);
                            // await this._DBContext.SaveChangesAsync();
                        }
                        processcount++;
                        if (processcount > 0)
                        {
                            await this._DBContext.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                            //  return new ResponseType() { KyValue = product.Rec_Id.ToString(), Result = "pass" };
                            response.Result = "pass";
                            response.ErrorMessage = "Data saved successfully";
                            response.responseCode = 200;
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                        }
                        //if (product.Variants != null && product.Variants.Count > 0)
                        //{
                        //    product.Variants.ForEach(item =>
                        //    {
                        //        var _resp = SaveProductVariant(item, product.Code);
                        //        if (_resp.Result)
                        //        {
                        //            processcount++;
                        //        }
                        //    });
                        //    if (processcount == product.Variants.Count)
                        //    {
                        //        await this._DBContext.SaveChangesAsync();
                        //        await dbtransaction.CommitAsync();
                        //        return new ResponseType() { KyValue = product.Code, Result = "pass" };
                        //    }
                        //    else
                        //    {
                        //        await dbtransaction.RollbackAsync();
                        //    }
                        //}
                    }
                }
                else
                {
                    response.Result = "fail";
                    response.ErrorMessage = "Product missing";
                    response.responseCode = 400;
                }
            }
            catch (Exception ex)
            {
                response.Result = "fail";
                response.ErrorMessage = "Failed to save";
                response.responseCode = 500;
            }
            return response;
            // return new ResponseType() { KyValue = string.Empty, Result = "fail" };
        }

        public async Task<APIResponse> RemoveProduct(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                //TblCustomer _customer = this.mapper.Map<Customermodal, TblCustomer>(data);
                //await this.context.TblCustomers.AddAsync(_customer);
                //await this.context.SaveChangesAsync();
                var _cat = await this._DBContext.TblProducts.FindAsync(code);
                if (_cat != null)
                {
                    this._DBContext.TblProducts.Remove(_cat);
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
    }
}
