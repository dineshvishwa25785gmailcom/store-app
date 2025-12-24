namespace store_app_apis.Repos.Models
{
    public class InvoiceFlatDto
    {
        // Company fields
        public string CoName { get; set; }
        public string CoAddr { get; set; }
        public string CoGST { get; set; }
        public string CntryCode { get; set; }
        public string CntryName { get; set; }
        public string StCode { get; set; }
        public string StName { get; set; }
        public string AccNum { get; set; }
        public string IFSC { get; set; }
        public string AccAddr { get; set; }
        public string CoEmail { get; set; }
        public string CoMob { get; set; }
        public string CoAltMob { get; set; }
        public string CoStatus { get; set; }

        // Customer fields
        public string CuName { get; set; }
        public string CuEmail { get; set; }
        public string CuPhone { get; set; }
        public bool CuStatus { get; set; }
        public string CuAddr { get; set; }
        public string CuCntryCode { get; set; }
        public string CuCntryName { get; set; }
        public string CuStCode { get; set; }
        public string CuStName { get; set; }
        public string CuMob { get; set; }
        public string CuAltMob { get; set; }

        // Invoice fields
        public string InvYear { get; set; }
        public string InvNum { get; set; }
        public DateTime? InvDate { get; set; }
        public string CoID { get; set; }
        public string CuID { get; set; }
        public string Dest { get; set; }
        public string DispThrough { get; set; }
        public string DelNote { get; set; }
        public string Remark { get; set; }
        public decimal? TotalAmt { get; set; }

    }
}
