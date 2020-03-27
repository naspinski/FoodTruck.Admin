using Naspinski.Messaging.Email;
using System.Threading.Tasks;

namespace Naspinski.FoodTruck.AdminWeb.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class AdminEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string apiKey, string fromEmail, string toEmail, string subject, string message)
        {
            EmailSender.Send(apiKey, subject, message, toEmail, fromEmail);
            return Task.CompletedTask;
        }
    }
}
