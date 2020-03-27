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
    public class ComboPartController : _BaseFoodTruckController
    {
        private ICrudHandler<ComboPartModel, FoodTruckContext, ComboPartModel> _handler;

        public ComboPartController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new ComboPartHandler(_context, _user);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _handler.Delete(id);
            return NoContent();
        }
    }
}
