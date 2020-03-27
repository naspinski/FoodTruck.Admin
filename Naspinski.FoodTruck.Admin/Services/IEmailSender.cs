using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naspinski.FoodTruck.AdminWeb.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string apiKey, string fromEmail, string toEmail, string subject, string message);
    }
}
