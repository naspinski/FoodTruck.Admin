using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.AdminWeb.Models;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Menu;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;
using Naspinski.FoodTruck.Data.Distribution.Models.Menu;
using Naspinski.FoodTruck.Data.Distribution.Models.System;
using System.Linq;
using System.Text.Json;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class MenuController : _BaseFoodTruckController
    {
        private MenuItemHandler _handler;
        private ICrudHandler<CategoryModel, FoodTruckContext, CategoryModel> _categoryHandler;
        private ICrudHandler<PriceTypeModel, FoodTruckContext, PriceTypeModel> _priceTypeHandler;
        private ICrudHandler<ImageModel, FoodTruckContext, ImageModel> _imageHandler;

        public MenuController(FoodTruckContext context) : base(context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new MenuItemHandler(_context, _user);
            _categoryHandler = new CategoryHandler(_context, _user);
            _priceTypeHandler = new PriceTypeHandler(_context, _user);
            _imageHandler = new ImageHandler(_context, _user);
        }
        
        [HttpGet]
        public IActionResult Index(bool deleted = false)
        {
            ViewData["Deleted"] = deleted;
            return View(new MenuItemsModel(_handler.GetAll(deleted)));
        }
        
        [HttpGet]
        public IActionResult Get(int id = 0)
        {
            return Json(_handler.Get(id));
        }

        [HttpGet]
        public IActionResult Edit(int id = 0)
        {
            ViewData["Categories"] = _categoryHandler.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            var priceTypes = _priceTypeHandler.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            ViewData["PriceTypes"] = JsonSerializer.Serialize(priceTypes);
            ViewData["Images"] = _imageHandler.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            return View(new MenuItemModel() { Id = id });
        }

        [HttpPost]
        public JsonResult Edit(MenuItemIntakeModel model)
        {
            return Json(_handler.Upsert(model.ToMenuItemModel()));
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
