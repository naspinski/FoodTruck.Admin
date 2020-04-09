using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;
using Naspinski.FoodTruck.Data.Distribution.Models.System;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class SquareLocationController : _BaseFoodTruckController
    {
        private ICrudHandler<SquareLocationModel, FoodTruckContext, SquareLocationModel> _handler;

        public SquareLocationController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new SquareLocationHandler(_context, _user);
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
        public IActionResult Edit(SquareLocationModel model)
        {
            return Json(_handler.Upsert(model));
        }
    }
}
