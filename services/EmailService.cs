using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UdemyApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var emailConfig = _configuration.GetSection("EmailConfiguration");
            var smtpServer = emailConfig["SmtpServer"];
            var port = int.Parse(emailConfig["Port"] ?? "587");
            var senderEmail = emailConfig["SenderEmail"];
            var senderName = emailConfig["SenderName"];
            var username = emailConfig["Username"];
            var password = emailConfig["Password"];

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail))
            {
                // In a real app, you might throw or log here.
                return; 
            }

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
