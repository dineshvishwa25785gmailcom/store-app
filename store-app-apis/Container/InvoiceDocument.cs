using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using store_app_apis.Models;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using static QuestPDF.Helpers.Colors;


namespace store_app_apis.Container
{
    public class InvoiceDocument : IDocument
    {
        // public static Image LogoImage { get; } = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "common", "logo.png"));
        TextStyle defaultTextStyle = TextStyle.Default.FontSize(10);
        public TblInvoiceHeader Model { get; }

        public InvoiceDocument(TblInvoiceHeader model)
        {
            Model = model;

        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(35);
                    //page.PageColor(Colors.White);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContentSellerInvoiceDetails);
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                        text.DefaultTextStyle(defaultTextStyle);
                    });
                });
        }

        //byte[] GetLogoByteArray()
        //{
        //    var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "common", "logo.png");
        //    return File.ReadAllBytes(logoPath);  // ✅ Read image as a byte array
        //}

        byte[] GetLogoByteArray()
        {
            try
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "common", "logo.png");

                return File.Exists(logoPath)
                    ? File.ReadAllBytes(logoPath)
                    : throw new FileNotFoundException($"Logo file missing: {logoPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading logo: {ex.Message}");
                return Array.Empty<byte>(); // Return an empty array to prevent crashes
            }
        }

        // Convert QuestPDF Image to byte array
        //byte[] ConvertImageToByteArray(SKImage skImage)
        //{
        //    using (var data = skImage.Encode(SKEncodedImageFormat.Png, 100))
        //    {
        //        return data.ToArray();
        //    }
        //}

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column
                        .Item().Text($"Invoice #{Model.InvoiceNumber}")
                        .FontSize(12).SemiBold().FontColor(Colors.Blue.Medium);
                    column.Item().Text(text =>
                    {
                        text.Span("Invoice date: ").SemiBold();
                        text.Span($"{Model.InvoiceDate:d}");
                        text.DefaultTextStyle(defaultTextStyle);
                    });
                });
                
                // Load logo if available
                try
                {
                    byte[] imageBytes = GetLogoByteArray();
                    if (imageBytes.Length > 0)
                    {
                        row.ConstantItem(120).Height(50).Width(100).Image(imageBytes, ImageScaling.FitWidth);
                    }
                }
                catch (Exception ex)
                {
                    // Logo not available - continue without it
                    System.Diagnostics.Debug.WriteLine($"Logo loading failed: {ex.Message}");
                }
            });
        }

        void ComposeContentSellerInvoiceDetails(IContainer container)
        {
            container.PaddingVertical(10).Border(2f) // Set border width
                                         .BorderColor(Colors.Grey.Lighten4) // Set border color
                                         .Column(column =>
{
    // 1st row (ComposeContentSeller)
    column.Item().Row(row =>
    {
        row.RelativeItem(0.7f).Container()
            .Background(Colors.White)
            .Padding(5).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentSeller);
        //row.RelativeItem(0.3f).Container()

        // Right box (30% of the width)
        row.RelativeItem(0.3f).Container()
            .Background(Grey.Lighten5)
            .Padding(5).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentInvoiceDetails);
    });
    // 2nd row (ComposeContentCustomer on a new line)
    column.Item().Row(row =>
    {
        row.RelativeItem(0.7f).Container()
            .Background(Colors.White)
            .Padding(3).DefaultTextStyle(defaultTextStyle)
            .Element(c => ComposeContentCustomer(c));
        // Right box (30% of the width)
        row.RelativeItem(0.3f).Container()
            .Background(Grey.Lighten5)
             .Padding(5).DefaultTextStyle(defaultTextStyle);
    });
    // 3rd row (ComposeContentCustomer on a new line)
    column.Item().Row(row =>
    {
        row.RelativeItem(1.0f).Container()
            .Background(Colors.White)
            .Padding(3).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentTables);

    });
    // 4th row (ComposeContentCustomer on a new line)
    column.Item().Row(row =>
    {
        row.RelativeItem(1.0f).Container()
            .Background(Colors.White)
            .Padding(3).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentGrandTotal);
    });
    // 5th row (ComposeContentCustomer on a new line)
    column.Item().Row(row =>
    {
        row.RelativeItem(1.0f).Container()
            .Background(Colors.White)
            .Padding(3).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentTaxDetails);
    });
    // 6th row (ComposeContentCustomer on a new line)
    column.Item().Row(row =>
    {
        row.RelativeItem(1.0f).Container()
            .Background(Colors.White)
            .Padding(3).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentAccountDetails);
    });
    // 7th row (ComposeContentCustomer on a new line)
    column.Item().Row(row =>
    {
        row.RelativeItem(1.0f).Container()
            .Background(Colors.White)
            .Padding(3).DefaultTextStyle(defaultTextStyle)
            .Element(ComposeContentDeclarationAndSignatory);
    });

});
        }
        void ComposeContentSeller(IContainer container)
        {
            container.PaddingVertical(2).Column(column =>
            {
                column.Spacing(3);
                if (Model.Company != null)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Component(new AddressComponent("From", Model.Company));
                    });
                }
                else
                {
                    column.Item().Text("Company information not available").FontSize(9).FontColor(Colors.Grey.Darken2);
                }
            });
        }
        void ComposeContentInvoiceDetails(IContainer container)
        {
            container.PaddingVertical(2).Column(column =>
            {
                column.Spacing(3);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new InvoiceComponent("Dispatch Details ", Model.InvoiceYear, Model.DeliveryNote ?? "N/A", Model.DispatchedThrough ?? "N/A", Model.Remark ?? "N/A"));
                });
            });
        }

        void ComposeContentCustomer(IContainer container)
        {
            container.PaddingVertical(2).Column(column =>
            {
                column.Spacing(3);
                if (Model.Customer != null)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Component(new AddressComponent("To", Model.Customer));
                    });
                }
                else
                {
                    column.Item().Text("Customer information not available").FontSize(9).FontColor(Colors.Grey.Darken2);
                }
            });
        }

        void ComposeContentTables(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                // ✅ Check if SalesItems collection has any items
                var items = Model.SalesItems ?? new List<TblSalesProductinfo>();
                
                if (items.Count > 0)
                {
                    // ✅ Render the table with sales items
                    column.Item().Element(c => ComposeTable(c, items.ToList()));
                }
                else
                {
                    // ✅ Show message if no items
                    column.Item()
                        .PaddingVertical(20)
                        .AlignCenter()
                        .Text("No items available")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken2);
                }
            });
        }
        void ComposeContentGrandTotal(IContainer container)
        {
            // Always calculate totals on-the-fly
            decimal totalTaxable = CalculateTotalAmount();
            decimal cgstAmount = CalculateCgstAmount();
            decimal sgstAmount = CalculateSgstAmount();
            decimal totalGst = cgstAmount + sgstAmount;
            decimal grandTotal = totalTaxable + totalGst;

            container.PaddingVertical(1f).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Labels (left side)
                    columns.RelativeColumn(1); // Amounts (right-aligned)
                });

                // Apply border and light shadow effect for the first three rows
                var borderedStyle = TextStyle.Default.FontSize(9).FontColor(Colors.Black);
                var borderedStylebold = TextStyle.Default.FontSize(11).FontColor(Colors.Black).SemiBold();

                table.Cell().Border(0.5f).BorderColor(Colors.Brown.Lighten5).Background(Colors.Brown.Lighten5)
                    .Padding(4).AlignRight().Text("Total Taxable Amount").Style(borderedStyle);
                table.Cell().Border(0.5f).BorderColor(Colors.Brown.Lighten4).Background(Colors.Brown.Lighten5)
                    .Padding(4).AlignRight().Text($"₹{totalTaxable:F2}").Style(borderedStyle);

                table.Cell().Border(0.5f).BorderColor(Colors.Brown.Lighten5).Background(Colors.Brown.Lighten5)
                    .Padding(4).AlignRight().Text("Total GST (CGST+SGST)").Style(borderedStyle);
                table.Cell().Border(0.5f).BorderColor(Colors.Brown.Lighten4).Background(Colors.Brown.Lighten5)
                    .Padding(4).AlignRight().Text($"₹{totalGst:F2}").Style(borderedStyle);

                table.Cell().Border(0.5f).BorderColor(Colors.Brown.Lighten5).Background(Colors.Brown.Lighten5)
                    .Padding(4).AlignRight().Text("Total Amount Chargeable").Style(borderedStylebold);
                table.Cell().Border(0.5f).BorderColor(Colors.Brown.Lighten4).Background(Colors.Brown.Lighten5)
                    .Padding(4).AlignRight().Text($"₹{grandTotal:F2}").Style(borderedStylebold);

                // **Last row should be full width & right-aligned**
                table.Cell().ColumnSpan(2).Padding(5).AlignRight().Text($"Total Amount Chargeable (in words): {InvoiceUtility.ConvertAmountToWords(grandTotal)}").FontSize(8).SemiBold();
            });
        }

        void ComposeContentTaxDetails(IContainer container)
        {
            // Calculate tax amounts using consistent methods
            decimal cgstAmount = CalculateCgstAmount();
            decimal sgstAmount = CalculateSgstAmount();
            decimal totalGst = cgstAmount + sgstAmount;

            container.PaddingVertical(3f).Row(row =>
            {
                // Left side (50%): Tax Amount in Words
                row.RelativeItem(0.5f).Container()
                    .Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                    .Text($"Tax Amount (in words): {InvoiceUtility.ConvertAmountToWords(totalGst)}")  // ✅ Dynamically bind tax amount in words
                    .SemiBold().FontSize(8f);

                // Right side (50%): Tax Details Table
                row.RelativeItem(0.5f).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1f); // Central Tax
                        columns.RelativeColumn(1f); // State Tax
                        columns.RelativeColumn(1f); // Total Tax Amount
                    });

                    // Header Row - Tax Categories
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text("Central Tax").SemiBold().FontSize(8f);
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text("State Tax").SemiBold().FontSize(8f);
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text("Total Tax Amount").SemiBold().FontSize(8f);

                    // Tax Rate Row (Dynamically Filled - calculate average rate from products)
                    decimal avgCgstRate = CalculateAverageCgstRate();
                    decimal avgSgstRate = CalculateAverageSgstRate();
                    
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text($"Rate: {avgCgstRate:F2}%").FontSize(8f);  // ✅ Dynamic CGST Rate
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text($"Rate: {avgSgstRate:F2}%").FontSize(8f);  // ✅ Dynamic SGST Rate

                    // Merged Cell for Total Tax Amount (RowSpan)
                    table.Cell().RowSpan(2).Container().Border(1f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text($"₹{totalGst:F2}").FontSize(8f);  // ✅ Calculated Total GST Amount

                    // Tax Amount Row (Dynamically Filled)
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text($"Amount: ₹{cgstAmount:F2}").FontSize(8f);  // ✅ Calculated CGST Amount
                    table.Cell().Container().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(3f)
                        .Text($"Amount: ₹{sgstAmount:F2}").FontSize(8f);  // ✅ Calculated SGST Amount
                });
            });
        }

        // Helper method to calculate total amount from line items
        private decimal CalculateTotalAmount()
        {
            if (Model.SalesItems == null || Model.SalesItems.Count == 0)
                return 0;
            
            decimal totalAmount = 0;
            foreach (var item in Model.SalesItems)
            {
                decimal qty = item.Quantity ?? 0;
                decimal rateWithTax = item.RateWithTax ?? 0;
                
                // Calculate rate without tax from rate with tax and product GST rate
                decimal rateWithoutTax = 0;
                if (item.Product?.TotalGstRate.HasValue == true && rateWithTax > 0)
                {
                    decimal gstPercent = item.Product.TotalGstRate.Value;
                    rateWithoutTax = rateWithTax / (1 + (gstPercent / 100));
                }
                
                totalAmount += qty * rateWithoutTax;
            }
            
            return totalAmount;
        }

        // Helper method to calculate average CGST rate from products
        private decimal CalculateAverageCgstRate()
        {
            if (Model.SalesItems == null || Model.SalesItems.Count == 0)
                return 0;

            var itemsWithCgstRate = Model.SalesItems.Where(si => si.Product?.CgstRate.HasValue == true).ToList();
            if (itemsWithCgstRate.Count == 0)
                return 0;

            return itemsWithCgstRate.Average(si => si.Product.CgstRate.Value);
        }

        // Helper method to calculate average SGST rate from products
        private decimal CalculateAverageSgstRate()
        {
            if (Model.SalesItems == null || Model.SalesItems.Count == 0)
                return 0;

            var itemsWithSgstRate = Model.SalesItems.Where(si => si.Product?.ScgstRate.HasValue == true).ToList();
            if (itemsWithSgstRate.Count == 0)
                return 0;

            return itemsWithSgstRate.Average(si => si.Product.ScgstRate.Value);
        }

        // Helper method to calculate CGST amount
        private decimal CalculateCgstAmount()
        {
            if (Model.SalesItems == null || Model.SalesItems.Count == 0)
                return 0;

            // Calculate CGST from each line item's product
            decimal cgstTotal = 0;
            foreach (var item in Model.SalesItems)
            {
                if (item.Product?.CgstRate.HasValue == true)
                {
                    decimal qty = item.Quantity ?? 0;
                    decimal rateWithTax = item.RateWithTax ?? 0;
                    
                    // Calculate rate without tax from rate with tax and product GST rate
                    decimal rateWithoutTax = 0;
                    if (item.Product.TotalGstRate.HasValue && rateWithTax > 0)
                    {
                        decimal gstPercent = item.Product.TotalGstRate.Value;
                        rateWithoutTax = rateWithTax / (1 + (gstPercent / 100));
                    }
                    
                    decimal cgst = (qty * rateWithoutTax) * (item.Product.CgstRate.Value / 100);
                    cgstTotal += cgst;
                }
            }

            return cgstTotal;
        }

        // Helper method to calculate SGST amount
        private decimal CalculateSgstAmount()
        {
            if (Model.SalesItems == null || Model.SalesItems.Count == 0)
                return 0;

            // Calculate SGST from each line item's product
            decimal sgstTotal = 0;
            foreach (var item in Model.SalesItems)
            {
                if (item.Product?.ScgstRate.HasValue == true)  // Note: Using ScgstRate (SGST)
                {
                    decimal qty = item.Quantity ?? 0;
                    decimal rateWithTax = item.RateWithTax ?? 0;
                    
                    // Calculate rate without tax from rate with tax and product GST rate
                    decimal rateWithoutTax = 0;
                    if (item.Product.TotalGstRate.HasValue && rateWithTax > 0)
                    {
                        decimal gstPercent = item.Product.TotalGstRate.Value;
                        rateWithoutTax = rateWithTax / (1 + (gstPercent / 100));
                    }
                    
                    decimal sgst = (qty * rateWithoutTax) * (item.Product.ScgstRate.Value / 100);
                    sgstTotal += sgst;
                }
            }

            return sgstTotal;
        }
        void ComposeContentAccountDetails(IContainer container)
        {
            container.PaddingVertical(3f).Row(row =>
            {
                row.Spacing(5f); // Adds some spacing before the right-aligned section

                // Empty space (Left 60%)
                row.RelativeItem(0.6f);

                // Account Details Box (Right 40%)
                row.RelativeItem(0.4f).Container()
                    .Border(0.5f).BorderColor(Colors.Grey.Lighten4).Padding(4f)
                      .BorderColor(Colors.Grey.Lighten4) // Lighter border color

                    .Column(column =>
                    {
                        column.Item().Text("Account Details").SemiBold().FontSize(8f);
                        column.Item().BorderBottom(1f).BorderColor(Colors.Grey.Lighten4).PaddingBottom(5f);
                        column.Item().Text($"Account No: {Model.Company.AccountNumber ?? "N/A"}").FontSize(10f);  // ✅ Dynamically bind Account Number
                        column.Item().Text($"IFSC Code: {Model.Company.Ifsc ?? "N/A"}").FontSize(8f);  // ✅ Dynamically bind IFSC Code
                        column.Item().Text($"Address: {Model.Company.AddressDetails ?? "N/A"}").FontSize(8f);  // ✅ Dynamically bind Address

                    });
            });
        }

        void ComposeContentDeclarationAndSignatory(IContainer container)
        {
            container.PaddingVertical(5f).Column(column =>
            {
                // Horizontal Line Separator
                column.Item().Container().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten4).PaddingBottom(5f);

                column.Item().Row(row =>
                {
                    // Left side (50%): Declaration
                    row.RelativeItem(0.5f).Container().Padding(5f)
                        .Column(declaration =>
                        {
                            declaration.Item().Text("Declaration").SemiBold().FontSize(10f);
                            declaration.Item().PaddingBottom(5f);

                            declaration.Item().Text("1. All disputes and claims shall be settled under the jurisdiction of Ayodhya Court.")
                                .FontSize(7f);
                            declaration.Item().Text("2. Goods sold will not be taken back, but can be exchanged if in safe condition.")
                                .FontSize(7f);
                            declaration.Item().Text("3. We are not responsible for any damage or loss during transportation.")
                                .FontSize(7f);
                        });

                    // Right side (50%): Authorised Signatory
                    row.RelativeItem(0.5f).Container().Padding(5f)
                        .Column(signatory =>
                        {
                            signatory.Item().AlignRight().Text("Authorised Signatory").SemiBold().FontSize(10f);
                            signatory.Item().AlignRight().PaddingTop(15f).Text("(Signature & Seal)").FontSize(8f);
                        });
                });
            });
        }
        void ComposeTable(IContainer container, List<TblSalesProductinfo> items)
        {
            var headerStyle = TextStyle.Default.FontSize(9).FontColor(Colors.Black).SemiBold();
            var rowValuesStyle = TextStyle.Default.FontSize(8).FontColor(Colors.Black);

            container.PaddingVertical(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30); // Serial number column
                    columns.RelativeColumn(2.5f);  // Product Name + HSN/SAC combined
                    columns.RelativeColumn(0.8f);   // GST Rate
                    columns.RelativeColumn(0.8f);   // Quantity
                    columns.RelativeColumn(0.8f);   // Per (Unit)
                    columns.RelativeColumn(1f);   // Rate (w/o Tax)
                    columns.RelativeColumn(1f);   // Rate (w/Tax)
                    columns.RelativeColumn(1.2f);   // Taxable Amount
                    columns.RelativeColumn(1.2f);   // Total Amount
                });

                // Header row formatted correctly with full borders and background
                table.Header(header =>
                {
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("#").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).Text("Product & HSN/SAC").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("GST%").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("Unit").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Qty").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Rate (w/Tax)").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Amount").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Rate (w/o Tax)").Style(headerStyle);
                    header.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten4).Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Taxable Amt").Style(headerStyle);
                   
                });

                // Populate data dynamically from items
                if (items != null && items.Count > 0)
                {
                    foreach (var item in items.Select((value, index) => new { Index = index + 1, Value = value }))
                    {
                        decimal qty = item.Value.Quantity ?? 0;
                        decimal rateWithTax = item.Value.RateWithTax ?? 0;
                        decimal total = qty * rateWithTax;

                        // Get product details from master table
                        var product = item.Value.Product;
                        string productName = product?.ProductName ?? "N/A";
                        string hsn = product?.HsnSacNumber ?? "N/A";
                        string gstRate = product?.TotalGstRate.HasValue == true ? $"{product.TotalGstRate:F2}%" : "N/A";
                        string unit = product?.Measurement ?? "N/A";

                        // Calculate rate without tax from rate with tax and GST rate
                        decimal rateWithoutTax = 0;
                        if (product?.TotalGstRate.HasValue == true && rateWithTax > 0)
                        {
                            decimal gstPercent = product.TotalGstRate.Value;
                            rateWithoutTax = rateWithTax / (1 + (gstPercent / 100));
                        }
                        
                        // Calculate taxable amount
                        decimal taxableAmount = qty * rateWithoutTax;

                        // Serial number
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Index}").Style(rowValuesStyle);
                        
                        // Product Name & HSN/SAC combined in one column
                        table.Cell().Element(CellStyle).Column(col =>
                        {
                            col.Item().Text(productName).FontSize(8).SemiBold().FontColor(Colors.Black);
                            col.Item().Text($"(HSN/SAC: {hsn})").FontSize(7).FontColor(Colors.Grey.Darken2);
                        });

                        // GST Rate (from product master)
                        table.Cell().Element(CellStyle).AlignCenter().Text(gstRate).Style(rowValuesStyle);
                        // Per (Unit) - from product master
                        table.Cell().Element(CellStyle).AlignCenter().Text(unit).Style(rowValuesStyle);

                        // Quantity
                        table.Cell().Element(CellStyle).AlignRight().Text($"{qty:F3}").Style(rowValuesStyle);
                        // Rate (Inc. of Tax)
                        table.Cell().Element(CellStyle).AlignRight().Text($"₹{rateWithTax:F2}").Style(rowValuesStyle);


                        // Total Amount
                        table.Cell().Element(CellStyle).AlignRight().Text($"₹{total:F2}").Style(rowValuesStyle);
                        // Rate (Exc. of Tax)
                        table.Cell().Element(CellStyle).AlignRight().Text($"₹{rateWithoutTax:F2}").Style(rowValuesStyle);

                       

                        // Taxable Amount (from database or calculated)
                        table.Cell().Element(CellStyle).AlignRight().Text($"₹{taxableAmount:F2}").Style(rowValuesStyle);

                    }
                }
            });
        }

        // Styling function to apply borders and padding to every cell
        static IContainer CellStyle(IContainer container) =>
            container.Border(0.5f).BorderColor(Colors.Grey.Lighten4).PaddingVertical(5).PaddingHorizontal(5);








        public string GetBase64String()
        {
            string webRootPath = Directory.GetCurrentDirectory() + "\\wwwroot"; // Set manually
            string filepath = Path.Combine(webRootPath, "upload\\common", "logo.png");

            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException($"Logo file not found at {filepath}");
            }

            byte[] imgarray = File.ReadAllBytes(filepath);
            string base64 = Convert.ToBase64String(imgarray);
            return base64;
        }


    }





    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private CompanyDTONM Address { get; }
        private CustomerDTO CAddress { get; }
        private TblCustomer TblCAddress { get; }

        public AddressComponent(string title, CompanyDTONM address)
        {
            Title = title;
            Address = address;
        }
        public AddressComponent(string title, CustomerDTO address)
        {
            Title = title;
            CAddress = address;
        }
        public AddressComponent(string title, TblCustomer address)
        {
            Title = title;
            TblCAddress = address;
        }
        public void Compose(IContainer container)
        {
            container.ShowEntire().Column(column =>
            {
                if (Address != null)
                {
                    column.Spacing(2);
                    column.Item().Text(string.IsNullOrEmpty(Title) ? "NA" : Title).SemiBold();
                    column.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten4);

                    column.Item().Text(string.IsNullOrEmpty(Address.Name) ? "NA" : Address.Name);
                    column.Item().Text(string.IsNullOrEmpty(Address.AddressDetails) ? "NA" : Address.AddressDetails);
                    column.Item().Text($"GSTIN/UIN/Aadhar/Mobile: {(string.IsNullOrEmpty(Address.GstNumber) ? "NA" : Address.GstNumber)}");
                    column.Item().Text($"{Address.StateName ?? "NA"}, {Address.StateCode ?? "NA"}");
                    column.Item().Text($"Contact/Mobile: {(string.IsNullOrEmpty(Address.MobileNo) ? "NA" : Address.MobileNo)}, {(string.IsNullOrEmpty(Address.AlternateMobile) ? "NA" : Address.AlternateMobile)} | E-Mail: {(string.IsNullOrEmpty(Address.EmailId) ? "NA" : Address.EmailId)}");
                }
                else if (CAddress != null)
                {
                    column.Spacing(2);
                    column.Item().Text(string.IsNullOrEmpty(Title) ? "NA" : Title).SemiBold();
                    column.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten4);
                    column.Item().Text(string.IsNullOrEmpty(CAddress.customer_company) ? "NA" : CAddress.customer_company);
                    column.Item().Text(string.IsNullOrEmpty(CAddress.AddressDetails) ? "NA" : CAddress.AddressDetails);
                    column.Item().Text($"GSTIN/UIN/Aadhar/Mobile: {(string.IsNullOrEmpty(CAddress.gst_number) ? "NA" : CAddress.gst_number)}");
                    column.Item().Text($"{CAddress.StateName ?? "NA"}, {CAddress.StateCode ?? "NA"}");
                    column.Item().Text($"Contact/Mobile: {(string.IsNullOrEmpty(CAddress.MobileNo) ? "NA" : CAddress.MobileNo)}, {(string.IsNullOrEmpty(CAddress.AlternateMobile) ? "NA" : CAddress.AlternateMobile)} | E-Mail: {(string.IsNullOrEmpty(CAddress.Email) ? "NA" : CAddress.Email)}");
                    column.Item().Text($"Place of Supply: {(string.IsNullOrEmpty(CAddress.StateName) ? "NA" : CAddress.StateName)}");
                    
                }
                else if (TblCAddress != null)
                {
                    column.Spacing(2);
                    column.Item().Text(string.IsNullOrEmpty(Title) ? "NA" : Title).SemiBold();
                    column.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten4);
                    column.Item().Text(string.IsNullOrEmpty(TblCAddress.Name) ? "NA" : TblCAddress.Name);
                    column.Item().Text(string.IsNullOrEmpty(TblCAddress.AddressDetails) ? "NA" : TblCAddress.AddressDetails);
                    column.Item().Text($"GSTIN/UIN/Aadhar/Mobile: {(string.IsNullOrEmpty(TblCAddress.gst_number) ? "NA" : TblCAddress.gst_number)}");
                    column.Item().Text($"{TblCAddress.StateName ?? "NA"}, {TblCAddress.StateCode ?? "NA"}");
                    column.Item().Text($"Contact/Mobile: {(string.IsNullOrEmpty(TblCAddress.MobileNo) ? "NA" : TblCAddress.MobileNo)}, {(string.IsNullOrEmpty(TblCAddress.AlternateMobile) ? "NA" : TblCAddress.AlternateMobile)} | E-Mail: {(string.IsNullOrEmpty(TblCAddress.Email) ? "NA" : TblCAddress.Email)}");
                    column.Item().Text($"Place of Supply: {(string.IsNullOrEmpty(TblCAddress.StateName) ? "NA" : TblCAddress.StateName)}");
                }
            });
        }
    }
    public class InvoiceComponent : IComponent
    {
        private string Title { get; }
        private string InvoiceYear { get; }

        private string DeliveryNote { get; }

        private string DispatchedThrough { get; }
        private string ramark { get; }

        public InvoiceComponent(string title, string InvoiceYear, string DeliveryNote, string DispatchedThrough, string ramark)
        {
            this.Title = title;
            this.InvoiceYear = InvoiceYear;
            this.DeliveryNote = DeliveryNote;
            this.DispatchedThrough = DispatchedThrough;
            this.ramark = ramark;
        }
        public void Compose(IContainer container)
        {
            container.ShowEntire().Column(column =>
            {
                column.Spacing(2);
                column.Item().Text(Title ?? "NA").SemiBold();
                column.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten4);

                column.Item().Text($"Invoice Year: {(string.IsNullOrEmpty(InvoiceYear) ? "NA" : InvoiceYear)}");
                column.Item().Text($"Delivery Note: {(string.IsNullOrEmpty(DeliveryNote) ? "NA" : DeliveryNote)}");
                column.Item().Text($"Dispatched Through: {(string.IsNullOrEmpty(DispatchedThrough) ? "NA" : DispatchedThrough)}");
                column.Item().Text($"Remark: {(string.IsNullOrEmpty(ramark) ? "NA" : ramark)}");
            });
        }
    }
}
