using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Naspinski.FoodTruck.Data;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [Authorize]
    public class HomeController : _BaseFoodTruckController
    {
        public HomeController(FoodTruckContext context) : base(context) { }

        public IActionResult Index()
        {
            return View();
        }
    }
}