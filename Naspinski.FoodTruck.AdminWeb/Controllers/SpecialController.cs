using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Specials;
using Naspinski.FoodTruck.Data.Distribution.Models.Specials;
using System;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class SpecialController : _BaseFoodTruckController
    {
        private ICrudHandler<SpecialModel, FoodTruckContext, SpecialModel> _handler;

        public SpecialController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new SpecialHandler(_context, _user);
        }

        [HttpGet]
        public IActionResult Index(bool deleted = false)
        {
            ViewData["Deleted"] = deleted;
            return View(_handler.GetAll(deleted));
        }

        [HttpGet]
        public IActionResult Edit(int id = 0)
        {
            return View(_handler.Get(id));
        }

        [HttpDelete]
        public IActionResult Delete(int id, bool isRestore = false)
        {
            if (isRestore) _handler.Restore(id);
            else _handler.Delete(id);
            return NoContent();
        }

        [HttpPost]
        public IActionResult Edit(SpecialModel model)
        {
            model.Begins = StringToTimeSpan(Request, "Begins");
            model.Ends = StringToTimeSpan(Request, "Ends");

            return Json(_handler.Upsert(model));
        }

        private TimeSpan? StringToTimeSpan(HttpRequest request, string key)
        {
            var timeObj = request.Form[key];
            if (string.IsNullOrEmpty(timeObj.ToString())) 
                return (TimeSpan?)null;

            var timeParts = timeObj.ToString().Split(' ');
            var time = TimeSpan.Parse(timeParts[0]);
            if (timeParts[1].ToUpper().Equals("PM"))
                time = time.Add(new TimeSpan(12, 0, 0));
            return time;
        }
    }
}
