using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Menu;
using Naspinski.FoodTruck.Data.Distribution.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class PriceController : _BaseFoodTruckController
    {
        private ICrudHandler<PriceModel, FoodTruckContext, PriceModel> _handler;

        public PriceController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new PriceHandler(_context, _user);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _handler.Delete(id);
            return NoContent();
        }
    }
}
