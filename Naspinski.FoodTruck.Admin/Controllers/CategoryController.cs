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
    public class CategoryController : _BaseFoodTruckController
    {
        private ICrudHandler<CategoryModel, FoodTruckContext, CategoryModel> _handler;

        public CategoryController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new CategoryHandler(_context, _user);
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

        [HttpPost]
        public IActionResult Edit(CategoryModel model)
        {
            return Json(_handler.Upsert(model));
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _handler.Delete(id);
            return NoContent();
        }
    }
}
