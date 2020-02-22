using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.AdminWeb.Models;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Events;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Menu;
using Naspinski.FoodTruck.Data.Distribution.Models.Events;
using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Naspinski.FoodTruck.Data.Constants;
using Command = Naspinski.FoodTruck.Data.Access.Commands;
using Query = Naspinski.FoodTruck.Data.Access.Queries;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class SettingsController : _BaseFoodTruckController
    {
        private ICrudHandler<ImageModel, FoodTruckContext, ImageModel> _imageHandler;
        private ICrudHandler<LocationModel, FoodTruckContext, LocationModel> _locationHandler;

        public SettingsController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _imageHandler = new ImageHandler(_context, _user);
            _locationHandler = new LocationHandler(_context, _user);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var _model = new Query.Settings.Get(_context).ExecuteAndReturnResults().Select(x => new Models.Settings.SettingModel(x));
            var model = new List<Models.Settings.SettingModel>();
            foreach (var s in Seeds.Settings.Select(x => x.Name))
            {
                var setting = _model.FirstOrDefault(x => x.Name == s);
                if (setting != null) model.Add(setting);
            }

            ViewData["Images"] = _imageHandler.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Location });
            ViewData["LocationList"] = _locationHandler.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            return View(model);
        }

        [HttpPut]
        public IActionResult Index(IEnumerable<Models.Settings.SettingModel> models)
        {
            models = Request.Form.Keys.Select(x => new Models.Settings.SettingModel() { Name = x, Value = Request.Form[x] });
            new Command.Settings.Update(_context, User.Identity.Name, models.Select(x => x.ToModel())).Execute();
            return NoContent();            
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult SwitchOrdering(string turn, string returnUrl)
        {
            var setting = new Models.Settings.SettingModel() { Name = SettingName.IsOrderingOn, Value = turn.ToUpper() == "ON" ? true.ToString() : false.ToString() };
            new Command.Settings.Update(_context, User.Identity.Name, setting.ToModel()).Execute();
            return Redirect($"https://{returnUrl}");
        }
    }
}
