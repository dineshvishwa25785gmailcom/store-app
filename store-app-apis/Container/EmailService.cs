using Microsoft.Extensions.Options;
using store_app_apis.Modal;
using store_app_apis.Service;
using MimeKit;
using MailKit.Net.Smtp; // Add this using directive
//using System.Net.Mail;
using MailKit.Security;
using Serilog;
namespace store_app_apis.Container
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            this._emailSettings = emailSettings.Value;
        }
        public async Task SendEmail(Mailrequest mailrequest)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailSettings.Email);
                email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
                email.Subject = mailrequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailrequest.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                //smtp.Dispose();

                //Log.Information("Email sent successfully to {ToEmail}", mailrequest.ToEmail);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending email to {ToEmail}", mailrequest.ToEmail);
                throw;
            }
        }
    }
}
