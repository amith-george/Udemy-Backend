using System.Threading.Tasks;

namespace UdemyApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
