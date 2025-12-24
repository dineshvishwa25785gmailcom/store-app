using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using store_app_apis.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using store_app_apis.Repos.Models;
using store_app_apis.Service;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using store_app_apis.Container;
using QuestPDF.Helpers;
using store_app_apis.Repos.Models.InvoiceListWithItems;
using store_app_apis.Modal;


namespace store_app_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;


        private readonly IInvoiceContainer _container;
        private readonly IWebHostEnvironment environment;
        private readonly Repos.StoreAppContext context;
        public InvoiceController(InvoiceService invoiceService,IInvoiceContainer container, Repos.StoreAppContext context, IWebHostEnvironment webHostEnvironment, ILogger<InvoiceController> logger)
        {
            this._container = container;
            this.environment = webHostEnvironment;
            this.context = context;
            this._invoiceService = invoiceService;
            this._logger = logger;
        }


        [HttpGet("GetAllInvoiceHeadersOnly")]
        public async Task<IActionResult> GetAllInvoiceHeadersOnly()
        {
            try
            {
                this._logger.LogInformation("Fetching all invoices...");

                var result = await this._container.GetAllInvoiceHeadersOnly(); // ✅ Calls service method to get all invoices

                if (result == null || result.Count == 0)
                {
                    this._logger.LogInformation("No invoices found.");
                    return Ok(new { message = "No invoices available." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error fetching invoices: {ex.Message}", ex);
                return StatusCode(500, new { error = "An unexpected error occurred. Please try again later." });
            }
        }




        [HttpGet("GetAllHeaderByFilter")]
        public async Task<IActionResult> GetAllHeaderByFilter(string? year, DateTime? fromDate, DateTime? toDate, string? dateType)
        {
            try
            {
                // ✅ Ensure at least Year OR (From Date & To Date with InvoiceDate/CreateDate selection) is provided
                if (string.IsNullOrEmpty(year) && (!fromDate.HasValue || !toDate.HasValue))
                {
                    this._logger.LogWarning("Validation failed: Provide Year OR (From Date & To Date) with selected date type.");
                    return BadRequest(new { error = "Provide either Year OR From Date & To Date with selected date type (Invoice or Create)." });
                }

                this._logger.LogInformation($"Fetching Invoice Headers - Year: {year ?? "NULL"}, From: {fromDate}, To: {toDate}, Date Type: {dateType}");

                var result = await this._container.GetAllInvoiceHeaderByFilter(year, fromDate, toDate, dateType);

                if (result == null || result.Count == 0)
                {
                    this._logger.LogInformation("No invoices found for the provided filters.");
                    return Ok(new { message = "No invoices found for the given filters." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                this._logger.LogWarning($"Validation Error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Unexpected Error: {ex.Message}", ex);
                return StatusCode(500, new { error = "An unexpected error occurred. Please try again later." });
            }
        }
        [HttpGet("GetAllInvoiceHeaderOnlybyCode")]
        public async Task<Invoice_Header_DTO> GetAllInvoiceHeaderOnlybyCode(string invoiceno)
        {
            return await this._container.GetAllInvoiceHeaderOnlybyCode(invoiceno);

        }

        [HttpGet("GetAllInvoiceCompCust")]
        public async Task<ActionResult<List<InvoiceFlatDto>>> GetAllInvoiceCompCust()
        {
            var result = await _container.GetAllInvoiceCompCust();
            return Ok(result);
        }


        [HttpGet("GetAllSalesItemsByCode")]
        public async Task<List<SalesProductDTO>> GetAllSalesItemsByCode(string invoiceno)
        {
            return await this._container.GetAllSalesItemsByCode(invoiceno);

        }



        [HttpPost("Save")]
        public async Task<ResponseType> Save([FromBody] InvoiceCreateDTO invoiceEntity)
        {
            return await this._container.Save(invoiceEntity);

        }

        [HttpDelete("Remove")]
        public async Task<ResponseType> Remove(string InvoiceNo)
        {
            return await this._container.Remove(InvoiceNo);

        }

        [HttpGet("{InvoiceNo}/pdf")]
        public async Task<IActionResult> GeneratePdf(string InvoiceNo)
        {
            try
            {
                this._logger.LogInformation("GeneratePdf");
                var pdfBytes = await _invoiceService.GenerateInvoicePdf(InvoiceNo);
                if (pdfBytes == null) return NotFound();

                return File(pdfBytes, "application/pdf", $"Invoice_{InvoiceNo}.pdf");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in GeneratePdf:" + ex.Message.ToString());
                return StatusCode(500, "Internal error while generating PDF");
            }
        }


      




          

        [NonAction]
        public string Getbase64string()
        {
            string filepath = this.environment.WebRootPath + "\\upload\\common\\logo.png";
            byte[] imgarray = System.IO.File.ReadAllBytes(filepath);
            string base64 = Convert.ToBase64String(imgarray);
            return base64;
        }
    }
}
