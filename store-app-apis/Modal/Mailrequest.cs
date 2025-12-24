namespace store_app_apis.Modal
{
    public class Mailrequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
    }
}
