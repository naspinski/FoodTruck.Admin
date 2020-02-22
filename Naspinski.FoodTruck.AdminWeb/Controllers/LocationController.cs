using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Events;
using Naspinski.FoodTruck.Data.Distribution.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static Naspinski.FoodTruck.Data.Constants;
using Query = Naspinski.FoodTruck.Data.Access.Queries;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class LocationController : _BaseFoodTruckController
    {
        private ICrudHandler<LocationModel, FoodTruckContext, LocationModel> _handler;

        public LocationController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new LocationHandler(_context, _user);
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
            var states = new List<SelectListItem>();
            foreach (var state in Enum.GetValues(typeof(State))) states.Add(new SelectListItem() { Text = state.ToString(), Value = state.ToString() });
            ViewData["States"] = states;

            return View(_handler.Get(id));
        }

        [HttpPost]
        public IActionResult Edit(LocationModel model)
        {
            model.GoogleMapsApiKey = new Query.Settings.Get(_context, new[] { SettingName.GoogleMapsDeveloperApiKey }).ExecuteAndReturnResults().First().Value;
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
