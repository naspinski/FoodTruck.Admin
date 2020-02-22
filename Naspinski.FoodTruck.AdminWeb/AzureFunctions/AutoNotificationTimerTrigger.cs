using System;

namespace Naspinski.FoodTruck.AdminWeb.AzureFunctions
{
    public class AutoNotificationTimerTrigger
    {
        public static void Run(TimerInfo everyMinute, TraceWriter log)
        {
            using (var client = new System.Net.WebClient())
            {
                client.UploadString("https://foodtruck-adminweb.azurewebsites.net/order/timernotification", "PUT", String.Empty);
            }
        }
    }
}
