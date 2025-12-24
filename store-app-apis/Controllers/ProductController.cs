using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace store_app_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _container;
        private readonly IWebHostEnvironment environment;
        private readonly Repos.StoreAppContext context;
        public ProductController(IWebHostEnvironment environment, Repos.StoreAppContext context, IProductService container)
        {
            this.environment = environment;
            this.context = context;
            _container = container;
        }
    
        [HttpGet("GetAll")]
        public async Task<List<ProductDTO>> GetAll()
        {
            return await this._container.Getall();
        }
        [HttpGet("GetByCode")]
        public async Task<ProductDTO> GetByCode(string Code)
        {
            return await this._container.Getbycode(Code);

        }

        [HttpGet("Getbyname")]
        public async Task<List<ProductDTO>> Getbyname(string name)
        {
            return await this._container.Getbyname(name);

        }


        [HttpPost("SaveProduct")]
        public async Task<APIResponse> SaveProduct([FromBody] ProductDTO _product)
        {
            return await this._container.SaveProduct(_product);
        }

        [HttpDelete("RemoveProduct")]
        public async Task<IActionResult> RemoveProduct(string code)
        {
            var data = await this._container.RemoveProduct(code);
            return Ok(data);
        }





        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productcode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilePath(productcode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.responseCode = 200;
                    response.Result = "pass";
                }
            }
            catch (Exception ex)
            {
                response.responseCode = 500;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                string Filepath = GetFilePath(productcode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }
                foreach (var file in filecollection)
                {
                    string imagepath = Filepath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;
                    }
                }
            }
            catch (Exception ex)
            {
                errorcount++;
                response.responseCode = 500;
                response.ErrorMessage = ex.Message;
            }
            response.responseCode = 200;
            response.Result = passcount + "File Uploaded &" + errorcount + "File Failed ";

            return Ok(response);
        }

        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                //string Filepath = GetFilePath(productcode);
                //if (!System.IO.Directory.Exists(Filepath))
                //{
                //    Directory.CreateDirectory(Filepath);
                //}
                foreach (var file in filecollection)
                {

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        this.context.TblProductimages.Add(new Repos.Models.TblProductimage() { Productcode = productcode, Productimage = memoryStream.ToArray() });
                        await this.context.SaveChangesAsync();
                        passcount++;
                    }
                    //string imagepath = Filepath + "\\" + file.FileName;
                    //if (System.IO.File.Exists(imagepath))
                    //{
                    //    System.IO.File.Delete(imagepath);
                    //}
                    //using (FileStream stream = System.IO.File.Create(imagepath))
                    //{
                    //    await file.CopyToAsync(stream);
                    //    passcount++;

                    //}
                }
            }
            catch (Exception ex)
            {
                errorcount++;
                response.responseCode = 500;
                response.ErrorMessage = ex.Message;
            }
            response.responseCode = 200;
            response.Result = passcount + "File Uploaded &" + errorcount + "File Failed ";
            return Ok(response);
        }
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productcode)
        {
            string Imageurl = string.Empty;
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    Imageurl = hosturl + "/upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(Imageurl);
        }
        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string productcode)
        {
            List<string> Imageurl = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(productcode);
                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryinfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfo = directoryinfo.GetFiles();
                    foreach (FileInfo fileInfo1 in fileInfo)
                    {
                        string filename = fileInfo1.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))
                        {
                            string _Imageurl = hosturl + "/upload/product/" + productcode + "/" + filename;
                            Imageurl.Add(_Imageurl);
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(Imageurl);
        }

        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string productcode)
        {
            List<string> Imageurl = new List<string>();
            // string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                var _productimage = this.context.TblProductimages.Where(item => item.Productcode == productcode).ToList();
                if (_productimage != null && _productimage.Count > 0)
                {
                    foreach (var item in _productimage)
                    {
                        string _Imageurl = "data:image/png;base64," + Convert.ToBase64String(item.Productimage);
                        Imageurl.Add(_Imageurl);
                    }
                }
                else
                {
                    return NotFound();
                }
                //string Filepath = GetFilePath(productcode);
                //if (System.IO.Directory.Exists(Filepath))
                //{
                //    DirectoryInfo directoryinfo = new DirectoryInfo(Filepath);
                //    FileInfo[] fileInfo = directoryinfo.GetFiles();
                //    foreach (FileInfo fileInfo1 in fileInfo)
                //    {
                //        string filename = fileInfo1.Name;
                //        string imagepath = Filepath + "\\" + filename;
                //        if (System.IO.File.Exists(imagepath))
                //        {
                //            string _Imageurl = hosturl + "/upload/product/" + productcode + "/" + filename;
                //            Imageurl.Add(_Imageurl);
                //        }
                //    }
                //}
                //else
                //{
                //    return NotFound();
                //}
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(Imageurl);
        }

        [HttpGet("downloadimage")]
        public async Task<IActionResult> download(string productcode)
        {
            // string Imageurl = string.Empty;
            // string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    using (FileStream stream = new FileStream(imagepath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memoryStream);
                    }
                    memoryStream.Position = 0;
                    return File(memoryStream, "application/octet-stream", productcode + ".png");
                    //Imageurl = hosturl + "/upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("dbdownloadimage")]
        public async Task<IActionResult> dbdownload(string productcode)
        {
            // string Imageurl = string.Empty;
            // string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                var _productimage = await this.context.TblProductimages.FirstOrDefaultAsync(item => item.Productcode == productcode);
                if (_productimage != null)
                {
                    return File(_productimage.Productimage, "application/octet-stream", productcode + ".png");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpDelete("removeimage")]
        public async Task<IActionResult> remove(string productcode)
        {
            // string Imageurl = string.Empty;
            // string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpDelete("multiremoveimage")]
        public async Task<IActionResult> multiremove(string productcode)
        {
            // string Imageurl = string.Empty;
            // string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(productcode);
                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryinfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfo = directoryinfo.GetFiles();
                    foreach (FileInfo fileInfo1 in fileInfo)
                    {
                        fileInfo1.Delete();
                    }
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [NonAction]
        private string GetFilePath(string productcode)
        {
            return this.environment.WebRootPath + "\\upload\\product\\" + productcode;
        }


        [NonAction]
        private string GetImagebyProduct(string productcode)
        {
            string ImageUrl = string.Empty;
            string HostUrl = "https://localhost:7118/";
            string Filepath = GetFilePath(productcode);
            string Imagepath = Filepath + "\\image.png";
            if (!System.IO.File.Exists(Imagepath))
            {
                ImageUrl = HostUrl + "/uploads/common/noimage.png";
            }
            else
            {
                ImageUrl = HostUrl + "/uploads/Product/" + productcode + "/image.png";
            }
            return ImageUrl;

        }












    }
}
