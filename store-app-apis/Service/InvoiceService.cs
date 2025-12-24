using QuestPDF.Fluent;
using store_app_apis.Container;

namespace store_app_apis.Service
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceService(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<byte[]> GenerateInvoicePdf(string invoiceNumber)
        {
            var invoice = await _invoiceRepository.GetInvoiceAsync("2025",invoiceNumber);
            if (invoice == null) return null;

            var document = new InvoiceDocument(invoice);
            return document.GeneratePdf();
        }
    }

}
