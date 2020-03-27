using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Naspinski.FoodTruck.AdminWeb.Services;

namespace Naspinski.FoodTruck.AdminWeb.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string apiKey, string toEmail, string fromEmail, string link)
        {
            return emailSender.SendEmailAsync(apiKey, fromEmail, toEmail, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
