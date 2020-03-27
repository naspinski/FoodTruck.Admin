using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.AdminWeb.Models.Files;
using Naspinski.FoodTruck.AdminWeb.Services;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.Menu;
using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class ImagesController : _BaseFoodTruckController
    {
        private ICrudHandler<ImageModel, FoodTruckContext, ImageModel> _handler;
        private readonly AzureSettings _azureSettings;

        public ImagesController(FoodTruckContext context, AzureSettings azureSettings) : base(context)
        {
            _azureSettings = azureSettings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new ImageHandler(_context, _user);
        }
        
        [HttpGet]
        public IActionResult Index(bool deleted = false)
        {
            ViewData["Deleted"] = deleted;
            return View(new ImageIndexModel() { Images = _handler.GetAll(deleted) });
        }

        [HttpPost]
        public async Task<IActionResult> Index(ImageIndexModel model)
        {
            var location = FileUploader.ProcessFormFile(_azureSettings.StorageAccount, _azureSettings.StorageAccountPassword, model.File, "images", ModelState).Result.ToString();
            _handler.Upsert(new ImageModel() { Name = model.Name, Location = location });
            return RedirectToAction(nameof(Index));
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
