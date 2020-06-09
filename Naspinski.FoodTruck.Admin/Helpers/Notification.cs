using Naspinski.FoodTruck.AdminWeb.Controllers;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Payment;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;
using Naspinski.FoodTruck.Data.Distribution.Models.Orders;
using Naspinski.Messaging.Email;
using Naspinski.Messaging.Sms;
using Naspinski.Messaging.Sms.Twilio;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Naspinski.FoodTruck.Data.Constants;

namespace Naspinski.FoodTruck.Admin.Helpers
{
    public static class Notification
    {
        public static void DoNotification(FoodTruckContext context, AdminAppSettings settings, AzureSettings azureSettings, OrderModel order, OrderHandler orderHandler = null)
        {
            orderHandler = orderHandler ?? new OrderHandler(context, "system");
            var isTextOn = new SettingHandler(context).Get(new[] { SettingName.IsTextOn }).FirstOrDefault().Value.ToString().ToUpper().Equals("true", StringComparison.OrdinalIgnoreCase);

            Parallel.Invoke(
                () => NotificationEmail(settings, azureSettings, order),
                () => { if (isTextOn) { NotificationText(settings, order); } }
            );
            orderHandler.Notify(order.Id);
        }

        public static void DoNotification(FoodTruckContext context, AdminAppSettings settings, AzureSettings azureSettings, int id, OrderHandler orderHandler = null)
        {
            orderHandler = orderHandler ?? new OrderHandler(context, "system");
            DoNotification(context, settings, azureSettings, orderHandler.Get(id, settings.TimeZoneOffsetFromUtcInHours), orderHandler);
        }

        private static string ReadyText(AdminAppSettings settings)
        {
            return $"Your {settings.Title} order is ready to be picked up!";
        }

        private static void NotificationEmail(AdminAppSettings settings, AzureSettings azureSettings, OrderModel order)
        {
            EmailSender.Send(azureSettings.SendgridApiKey,
                $"{settings.Title} - Order Ready",
                ReadyText(settings),
                order.Email, settings.ContactEmail);
        }

        private static void NotificationText(AdminAppSettings settings, OrderModel order)
        {
            if (!string.IsNullOrWhiteSpace(order.Phone) && settings.IsTwilioValid)
            {
                ISmsSender smsSender = new TwilioSmsSender(settings.TwilioSid, settings.TwilioAuthToken);
                smsSender.Send(settings.TwilioPhoneNumber, order.Phone, ReadyText(settings));
            }
        }
    }
}
