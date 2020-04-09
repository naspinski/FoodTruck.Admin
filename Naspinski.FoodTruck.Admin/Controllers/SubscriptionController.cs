using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Models.Events;
using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Naspinski.FoodTruck.Data.Models.System;
using Naspinski.Messaging.Email;
using Naspinski.Messaging.Sms;
using Naspinski.Messaging.Sms.Twilio;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Naspinski.FoodTruck.Data.Constants;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Route("subscription")]
    public class SubscriptionController : _BaseFoodTruckController
    {
        private readonly AzureSettings _azureSettings;
        private SubscriptionHandler _handler;
        private FoodTruck.Data.Distribution.Handlers.Events.EventHandler _eventHandler;

        private string _twilioAuthToken;
        private string _twilioSid;
        private string _twilioPhone;

        public SubscriptionController(FoodTruckContext context, AzureSettings azureSettings) : base(context)
        {
            _azureSettings = azureSettings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new SubscriptionHandler(_context);
            _eventHandler = new FoodTruck.Data.Distribution.Handlers.Events.EventHandler(_context, "system");
            
            var settings = new SettingHandler(_context).Get(new[]
            {
                SettingName.TwilioAuthToken,
                SettingName.TwilioSid,
                SettingName.TwilioPhoneNumber
            });

            _twilioAuthToken = settings.First(x => x.Name == SettingName.TwilioAuthToken).Value;
            _twilioSid = settings.First(x => x.Name == SettingName.TwilioSid).Value;
            _twilioPhone = settings.First(x => x.Name == SettingName.TwilioPhoneNumber).Value;
        }

        [HttpDelete]
        public IActionResult Unsubscribe(string subscriber, string location)
        {
            _handler.Delete(subscriber, location);
            return new StatusCodeResult((int)HttpStatusCode.NoContent);
        }

        [Route("test")]
        public IActionResult Test()
        {
            return Ok();
        }

        [Route("notify")]
        [HttpPut]
        public IActionResult Notify()
        {
            var json = string.Empty;
            using (var client = new System.Net.WebClient())
            {
                json = client.DownloadString($"{_azureSettings.SiteBaseUrl}api/events");
            }
            var events = JsonConvert.DeserializeObject<IEnumerable<EventModel>>(json).ToList();
            
            events = events.Where(x => x.Begins.Date == DateTime.Now.Date).ToList();

            var locations = events.Where(x => x.Location != null).Select(x => Subscription.SanitizeLocation(x.Location.Name)).Distinct().ToList();

            var subs = _handler.GetAll(null, locations).Where(x => !string.IsNullOrWhiteSpace(x.Location)).ToList();

            var sentTo = new List<string>();
            foreach(var sub in subs.Where(x => x.Type == Subscription.Types.Email))
            {
                if (!sentTo.Contains(sub.Subscriber))
                {
                    try
                    {
                        var n = Environment.NewLine;
                        var _event = GetEvent(events, sub.Location);
                        EmailSender.Send(_azureSettings.SendgridApiKey,
                            $"{_settings.Title} will be at {_event.Location.Name} today!",
                            SubscriptionMessage(_event, sub),
                            sub.Subscriber,
                            _settings.ContactEmail
                        );
                        sentTo.Add(sub.Subscriber);
                    }
                    catch (Exception ex) { ex.Ship(HttpContext); }
                }
            }

            var twilio = new TwilioHelper(_twilioAuthToken, _twilioSid, _twilioPhone);
            if (!twilio.IsValid)
                new Exception("twilio settings invalid").Ship(HttpContext);
            else
            {
                ISmsSender smsSender = new TwilioSmsSender(twilio);
                foreach (var sub in subs.Where(x => x.Type == Subscription.Types.Text))
                {
                    if (!sentTo.Contains(sub.Subscriber))
                    {
                        try
                        {
                            var _event = GetEvent(events, sub.Location);
                            smsSender.Send(twilio.Phone, $"+1{sub.Subscriber}", SubscriptionMessage(_event, sub));
                            sentTo.Add(sub.Subscriber);
                        }
                        catch (Exception ex) { ex.Ship(HttpContext); }
                    }
                }
            }

            return Ok();
        }

        private static EventModel GetEvent(List<EventModel> events, string location)
        {
            return events.FirstOrDefault(x => x.Location.Name.StartsWith(location, StringComparison.InvariantCultureIgnoreCase));
        }

        private string SubscriptionMessage(EventModel _event, SubscriptionModel sub)
        {
            var n = Environment.NewLine;
            return $"{_settings.Title} will be at {_event.Location.Name} today from {_event.BeginsTime} to { _event.EndsTime}{n}"
                        + $"{n}unsubscribe from this location: {_azureSettings.SiteBaseUrl}api/u/{sub.Subscriber}---{Uri.EscapeDataString(sub.Location)}"
                        + $"{n}unsubscribe from all locations: {_azureSettings.SiteBaseUrl}api/u/{Uri.EscapeDataString(sub.Subscriber)}/";
        }

        private void AzureFunction()
        {
            using (var client = new System.Net.WebClient())
            {
                client.UploadString("https://foodtruck-adminweb-dev.azurewebsites.net/subscription/notify", "PUT", String.Empty);
            }
        }

        private void Log(string message)
        {
            var _event = new LogEvent(message);
            _event.Ship(this.HttpContext);
        }
    }

    public class LogEvent : Exception
    {
        public LogEvent(string message) : base(message) { }

        internal void Ship()
        {
            throw new NotImplementedException();
        }
    }
}
