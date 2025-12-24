using store_app_apis.Modal;

namespace store_app_apis.Service
{
    public interface IEmailService
    {
        Task SendEmail(Mailrequest mailrequest);
    }
}
