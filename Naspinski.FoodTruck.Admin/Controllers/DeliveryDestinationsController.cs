using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.AdminWeb.Models.DeliveryDestination;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class DeliveryDestinationsController : _BaseFoodTruckController
    {
        private ICrudHandler<DeliveryDestinationModel, FoodTruckContext, DeliveryDestinationModel> _handler;

        public DeliveryDestinationsController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new DeliveryDestinationHandler(_context, _user);
        }

        [HttpGet]
        public IActionResult Index(bool deleted = false)
        {
            return View(new DeliveryDestinationsIndexModel(_handler.GetAll(deleted)));
        }

        [HttpPost]
        public IActionResult Index(DeliveryDestinationModel NewDestination)
        {
            Json(_handler.Upsert(NewDestination));
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _handler.Delete(id);
            return NoContent();
        }
    }
}
