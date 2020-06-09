using Elmah.Io.AspNetCore;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naspinski.FoodTruck.Admin.Helpers;
using Naspinski.FoodTruck.Admin.Models.SquareModels;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Models.Orders;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Route("square")]
    public class SquareController : _BaseFoodTruckController
    {
        private readonly AzureSettings _azureSettings;
        private FoodTruck.Data.Distribution.Handlers.Payment.OrderHandler _handler;

        public SquareController(FoodTruckContext context, AzureSettings azureSettings) : base(context)
        {
            _azureSettings = azureSettings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new FoodTruck.Data.Distribution.Handlers.Payment.OrderHandler(_context, _user);
        }

        [HttpPost]
        [Route("test")]
        public JsonResult Test([FromBody] JsonElement json)
        {
            new SquareWebHook(json.ToString()).Ship(this.HttpContext);
            return Json(json);
        }

        [HttpPost]
        [Route("payment")]
        public JsonResult Payment([FromBody] JsonElement json)
        {
            new SquareWebHook(json.ToString()).Ship(this.HttpContext);
            return Json(json);
        }

        [HttpPost]
        [Route("")]
        public ActionResult OrderReady([FromBody]SquareWebhookRequest model)
        {
            new SquareWebHook(JsonConvert.SerializeObject(model)).Ship(this.HttpContext);
            if(model?.Data?.Object?.OrderFullfillmentUpdated?.FullfillmentUpdate?[0] != null)
            {
                var fullfillment = model.Data.Object.OrderFullfillmentUpdated.FullfillmentUpdate[0];
                if(fullfillment.NewState != fullfillment.OldState && fullfillment.NewState.Equals("PREPARED", StringComparison.InvariantCultureIgnoreCase))
                {
                    var _order = _context.Orders.SingleOrDefault(x => x.SquareOrderId == model.Data.Object.OrderFullfillmentUpdated.OrderId);
                    if (_order != null)
                    {
                        _handler.Made(_order);
                        Notification.DoNotification(_context, _settings, _azureSettings, _order.Id, _handler);
                    }
                }
            }
            return Ok();
        }
    }

    public class SquareWebHook : Exception
    {
        public SquareWebHook(string ex) : base(ex) { }
    }
}
