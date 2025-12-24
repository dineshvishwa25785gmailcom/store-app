using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Container;
using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;

namespace store_app_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterContainer _container;
        private readonly ILogger<MasterContainer> _logger;
        public MasterController(IMasterContainer container, ILogger<MasterContainer> _logger)
        {
            this._container = container;
            this._logger = _logger;
        }

         
        [HttpGet("CatGetAll")]
        public async Task<List<CategoryDTO>> CatGetAll()
        {
            var productlist = await this._container.CatGetall();
            if (productlist != null && productlist.Count > 0)
            {
                productlist.ForEach(item =>
                {
                    //item.productImage = GetImagebyProduct(item.Code);
                });
            }
            else
            {
                return new List<CategoryDTO>();
            }
            return productlist;
        }
        [HttpGet("CatGetByCode")]
        public async Task<CategoryDTO> CatGetByCode(string UKID)
        {
            return await this._container.CatGetbycode(UKID);

        }

        [HttpGet("CatGetbycategory")]
        public async Task<List<CategoryDTO>> CatGetbycategory(string CategoryName)
        {
            return await this._container.CatGetbycategory(CategoryName);

        }

        [HttpPost("SaveCategory")]
        public async Task<APIResponse> SaveCategory([FromBody] CategoryDTO _category)
        {
            return await this._container.SaveCategory(_category);
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(string UKID)
        {
            var data = await this._container.Remove(UKID);
            return Ok(data);
        }






        //////////////////###########################@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@############################//////////////////
        [HttpGet("MeasurmentGetall")]
        public async Task<List<MeasurementDto>> MeasurmentGetall()
        {
            var productlist = await this._container.MeasurmentGetall();
            if (productlist != null && productlist.Count > 0)
            {
                productlist.ForEach(item =>
                {
                    //item.productImage = GetImagebyProduct(item.Code);
                });
            }
            else
            {
                return new List<MeasurementDto>();
            }
            return productlist;
        }






    }
}
