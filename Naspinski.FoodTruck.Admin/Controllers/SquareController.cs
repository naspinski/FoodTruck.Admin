using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naspinski.FoodTruck.Admin.Models.SquareModels;
using Naspinski.FoodTruck.Data;
using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Route("square")]
    public class SquareController : _BaseFoodTruckController
    {
        public SquareController(FoodTruckContext context) : base(context)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
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
            return Ok();
        }
    }

    public class SquareWebHook : Exception
    {
        public SquareWebHook(string ex) : base(ex) { }
    }
}
