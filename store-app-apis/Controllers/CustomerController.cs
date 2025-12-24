using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using store_app_apis.Modal;
using store_app_apis.Repos.Models;
using store_app_apis.Service;
using System.Data;
using System.IO;

namespace store_app_apis.Controllers
{
    //Note: this we need to enable before deployment ------------------ [Authorize]
    //If we want to disable the cors policy controller level, SO the perticular controller with the action method can not be accessed from any other domain or application: Simply we are blocking the access of the controller. Note: this is in the rare cases. 
    // Controller level CORS policy
    // [EnableCors("corspolicy1")]
   // [EnableRateLimiting("fixedwindow")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService service;
        private readonly IWebHostEnvironment  environment;
        public CustomerController(ICustomerService service, IWebHostEnvironment environment)
        {
            this.service = service;
            this.environment = environment;
        }
        // This is syncronuous programming  
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var data = this.service.Getall();
        //    if (data == null || data.Count == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(data);
        //}
        // This is Asynchronous Programming
        // If we want to impliment for a perticular action method 
       // [AllowAnonymous] // Site is not secure : It is just like a open API
        //[EnableCors("corspolicy1")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await this.service.Getall();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [DisableRateLimiting]
        [HttpGet("GetByUniqueKeyID")]
        public async Task<IActionResult> GetByUniqueKeyID(string code)
        {
            var data = await this.service.GetByUniqueKeyID(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CustomerDTO _data)
        {
            var data = await this.service.Create(_data);
            return Ok(data);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(CustomerDTO _data, string code)
        {
            var data = await this.service.Update(_data, code);
            return Ok(data);
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(string code)
        {
            var data = await this.service.Remove(code);
            return Ok(data);
        }
        [AllowAnonymous]
        [HttpGet("Exportexcel")]
        public async Task<IActionResult> Exportexcel()
        {
            try
            {
                string filepath = GetFilePath();
                string excelpath = filepath + "\\Customer.xlsx";
                DataTable dt = new DataTable();
                dt.Columns.Add("UniqueKeyID", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("IsActive", typeof(int));
                dt.Columns.Add("AddressDetails", typeof(string));
                dt.Columns.Add("CountryCode", typeof(string));
                dt.Columns.Add("CountryName", typeof(string));
                dt.Columns.Add("StateCode", typeof(string));
                dt.Columns.Add("StateName", typeof(string));
                dt.Columns.Add("MobileNo", typeof(string));
                dt.Columns.Add("AlternateMobile", typeof(string));
                dt.Columns.Add("CustomerCompany", typeof(string));
                dt.Columns.Add("GstNumber", typeof(string));
                dt.Columns.Add("CreateDate", typeof(DateTime));
                dt.Columns.Add("CreateIp", typeof(string));
                dt.Columns.Add("UpdateDate", typeof(DateTime));
                dt.Columns.Add("UpdateIp", typeof(string));
                var data = await this.service.Getall();
                if (data != null && data.Count > 0)
                {
                    data.ForEach(item =>
                    {
                        dt.Rows.Add(
                            item.UniqueKeyID,
                            item.Name,
                            item.Email,
                            item.Phone,
                            item.IsActive,
                            item.AddressDetails,
                            item.CountryName,
                            item.StateName,
                            item.MobileNo,
                            item.AlternateMobile,
                            item.customer_company,
                            item.gst_number,
                            item.CreateDate,
                            item.UpdateIp
                        );
                    });
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Customer");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        if (System.IO.File.Exists(excelpath))
                        {
                            System.IO.File.Delete(excelpath);
                        }
                        wb.SaveAs(excelpath);
                        stream.Close(); // Explicitly close the stream
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [NonAction]
        private string GetFilePath()
        {
            return this.environment.WebRootPath + "\\Export" ;
        }
    }
}
