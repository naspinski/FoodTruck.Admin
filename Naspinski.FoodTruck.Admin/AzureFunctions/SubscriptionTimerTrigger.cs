using System;

namespace Naspinski.FoodTruck.AdminWeb.AzureFunctions
{
    public class SubscriptionTimerTrigger
    {
        public static void Run(TimerInfo dailyAtSeven, TraceWriter log)
        {
            using (var client = new System.Net.WebClient())
            {
                client.UploadString("https://foodtruck-adminweb.azurewebsites.net/subscription/notify", "PUT", String.Empty);
            }
        }
    }
}
