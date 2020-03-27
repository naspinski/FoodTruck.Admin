using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Events;
using Naspinski.FoodTruck.Data.Distribution.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class EventController : _BaseFoodTruckController
    {
        private FoodTruck.Data.Distribution.Handlers.Events.EventHandler _handler;
        private ICrudHandler<LocationModel, FoodTruckContext, LocationModel> _locationHandler;

        public EventController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new FoodTruck.Data.Distribution.Handlers.Events.EventHandler(_context, _user);
            _locationHandler = new LocationHandler(_context, _user);
        }

        [HttpGet]
        public IActionResult Index(DateTime? from = null, DateTime? to = null, bool deleted = false)
        {
            var _from = from ?? DateTime.Now.AddDays(-1).AddHours(_settings.TimeZoneOffsetFromUtcInHours).Date;
            ViewData["From"] = _from;
            ViewData["To"] = to;
            ViewData["Deleted"] = deleted;
            return View(_handler.GetAll(_from, to, deleted));
        }

        [HttpGet]
        public IActionResult Edit(int id = 0)
        {
            ViewData["Locations"] = _locationHandler.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            return View(_handler.Get(id));
        }

        [HttpPost]
        public IActionResult Edit(EventModel model)
        {
            return Json(_handler.Upsert(model));
        }

        [HttpDelete]
        public IActionResult Delete(int id, bool isRestore = false)
        {
            if (isRestore) _handler.Restore(id);
            else _handler.Delete(id);
            return NoContent();
        }
    }
}
