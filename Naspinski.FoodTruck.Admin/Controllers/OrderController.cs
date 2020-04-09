using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Naspinski.FoodTruck.AdminWeb.SignalR;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;
using Naspinski.FoodTruck.Data.Distribution.Models.Orders;
using Naspinski.Messaging.Email;
using Naspinski.Messaging.Sms;
using Naspinski.Messaging.Sms.Twilio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Naspinski.FoodTruck.Data.Constants;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class OrderController : _BaseFoodTruckController
    {
        private FoodTruck.Data.Distribution.Handlers.Payment.OrderHandler _handler;
        private readonly AzureSettings _azureSettings;
        private IHubContext<SignalHub, ISignalHub> _hubContext;

        public OrderController(FoodTruckContext context, AzureSettings azureSettings, IHubContext<SignalHub, ISignalHub> hubContext) : base(context)
        {
            _azureSettings = azureSettings;
            _hubContext = hubContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new FoodTruck.Data.Distribution.Handlers.Payment.OrderHandler(_context, _user);
        }

        [HttpGet]
        public IActionResult Index(DateTime? from = null, DateTime? to = null, bool deleted = false, bool sandbox = false, bool showMade = false)
        {
            var _from = from ?? DateTime.UtcNow.AddHours(_settings.TimeZoneOffsetFromUtcInHours).Date;
            ViewData["From"] = _from;
            ViewData["To"] = to;
            ViewData["Deleted"] = deleted;
            ViewData["Sandbox"] = sandbox;
            ViewData["ShowMade"] = showMade;
            return View();
        }

        [HttpGet]
        public IEnumerable<OrderModel> Get(DateTime? from = null, DateTime? to = null, bool deleted = false, bool sandbox = false, bool showMade = false)
        {
            return _handler.GetAll(from.HasValue ? from.Value : DateTime.Now.Date, to, deleted, sandbox, showMade, _settings.TimeZoneOffsetFromUtcInHours)
                .OrderBy(x => x.Created);
        }
        
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _handler.Delete(id);
            return NoContent();
        }

        [HttpPut]
        public IActionResult Made(int id, bool notify = false)
        {
            var order = _handler.Made(id);
            if (notify) DoNotification(new OrderModel(order));
            return NoContent();
        }

        [HttpPut]
        public IActionResult Notify(int id) //order.Id
        {
            DoNotification(id);
            return NoContent();
        }
        
        private void DoNotification(OrderModel order)
        {
            var isTextOn = new SettingHandler(_context).Get(new[] { SettingName.IsTextOn }).FirstOrDefault().Value.ToString().ToUpper().Equals("true", StringComparison.OrdinalIgnoreCase);

            Parallel.Invoke(
                () => NotificationEmail(order),
                () => { if (isTextOn) { NotificationText(order); } }
            );
            _handler.Notify(order.Id);
        }

        private void DoNotification(int id)
        {
            DoNotification(_handler.Get(id, _settings.TimeZoneOffsetFromUtcInHours));
        }
        
        private string ReadyText
        {
            get
            {
                return $"Your {_settings.Title} order is ready to be picked up!";
            }
        }

        private void NotificationEmail(OrderModel order)
        {
            EmailSender.Send(_azureSettings.SendgridApiKey,
                $"{_settings.Title} - Order Ready",
                ReadyText,
                order.Email, _settings.ContactEmail);
        }

        private void NotificationText(OrderModel order)
        {
            if (!string.IsNullOrWhiteSpace(order.Phone) && _settings.IsTwilioValid)
            {
                ISmsSender smsSender = new TwilioSmsSender(_settings.TwilioSid, _settings.TwilioAuthToken);
                smsSender.Send(_settings.TwilioPhoneNumber, order.Phone, ReadyText);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public void TimerNotification(bool sandbox = false)
        {
            if (_settings.AutoNotificationDelayInMinutes < 1)
                return;

            var orders = _handler.GetAll(DateTime.Now.Date, null, false, sandbox, false, 0) //use UTC
                .OrderBy(x => x.Created).ToList();

            var sendNotificationsForOrdersMadeAfter = DateTime.UtcNow.AddMinutes(0 - _settings.AutoNotificationDelayInMinutes);
            var ordersPastDelayNotNotified = orders.Where(x => x.Notified == null && x.Created < sendNotificationsForOrdersMadeAfter).ToList();

            if (ordersPastDelayNotNotified.Any())
            {
                foreach (var order in ordersPastDelayNotNotified)
                    DoNotification(order.Id);

                _hubContext.Clients.All.Broadcast("notifications-sent");
            }
        }

        private void AzureFunction()
        {
            using (var client = new System.Net.WebClient())
            {
                client.UploadString("https://foodtruck-adminweb-dev.azurewebsites.net/order/timernotification", "PUT", String.Empty);
            }
        }
    }
}
