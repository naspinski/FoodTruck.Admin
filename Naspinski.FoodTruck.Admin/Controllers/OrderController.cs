using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Naspinski.FoodTruck.Admin.Helpers;
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
            if (notify) 
                Notification.DoNotification(_context, _settings, _azureSettings, new OrderModel(order), _handler);
            return NoContent();
        }

        [HttpPut]
        public IActionResult Notify(int id) //order.Id
        {
            Notification.DoNotification(_context, _settings, _azureSettings, id, _handler);
            return NoContent();
        }
    }
}
