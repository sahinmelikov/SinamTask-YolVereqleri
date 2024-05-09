using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SinamYolVereqi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
           Task<string> GetEmailPasswordAsync(string email);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public async Task<string> GetEmailPasswordAsync(string email)
        {
            // E-posta ile ilişkilendirilmiş şifreyi veritabanından al
            // Bu örnekte, basit bir örnek olduğu için sabit bir şifre döndüreceğiz.
            // Gerçek bir uygulamada, bu şifreyi güvenli bir şekilde saklamalısınız.
            return "YourEmailPassword"; // Gerçek şifreyi kendi uygulamanıza göre değiştirin
        }
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient())
            {
                var emailConfig = _configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
                client.Host = emailConfig.SmtpServer;
                client.Port = emailConfig.SmtpPort;
                client.EnableSsl = emailConfig.EnableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(emailConfig.SmtpUsername, emailConfig.SmtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailConfig.FromEmail, emailConfig.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }


    }

    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; }
    }
}
