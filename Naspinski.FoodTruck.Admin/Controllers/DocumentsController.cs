using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naspinski.Data.Interfaces;
using Naspinski.FoodTruck.AdminWeb.Models.Files;
using Naspinski.FoodTruck.AdminWeb.Services;
using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Distribution.Handlers.System;
using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Naspinski.FoodTruck.Data.Models.Storage;
using System.Threading.Tasks;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class DocumentsController : _BaseFoodTruckController
    {
        private ICrudHandler<DocumentModel, FoodTruckContext, DocumentModel> _handler;
        private readonly AzureSettings _azureSettings;

        public DocumentsController(FoodTruckContext context, AzureSettings azureSettings) : base(context)
        {
            _azureSettings = azureSettings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _handler = new DocumentHandler(_context, _user);
        }
        
        [HttpGet]
        public IActionResult Index(bool deleted = false)
        {
            ViewData["Deleted"] = deleted;
            return View(new DocumentIndexModel() { Documents = _handler.GetAll(deleted) });
        }

        [HttpPost]
        public async Task<IActionResult> Index(DocumentIndexModel model)
        {
            var title = $"{ViewData["Title"]} - {model.Category}".Replace(" ", "");
            var location = FileUploader.ProcessFormFile(_azureSettings.StorageAccount, _azureSettings.StorageAccountPassword, 
                model.File, "documents", ModelState, title, true).Result.ToString();

            _handler.Upsert(
                 new DocumentModel()
                 {
                     Location = location,
                     Type = Document.DocType.FromFileName(location),
                     Category = model.Category,
                     DeleteAllOthers = true
                 }
             );

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
