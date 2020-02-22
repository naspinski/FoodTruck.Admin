using Naspinski.FoodTruck.AdminWeb.SignalR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    [EnableCors("SignalRPolicy")]
    public class SignalController : Controller
    {
        private IHubContext<SignalHub, ISignalHub> _hubContext;
        private string[] _allowedBroadcastMessages = new[] { "new-order", "notifications-sent" };

        public SignalController(IHubContext<SignalHub, ISignalHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Broadcast(string message)
        {
            if (!_allowedBroadcastMessages.Contains(message))
                throw new ArgumentException();

            _hubContext.Clients.All.Broadcast(message);
            
            return Ok();
        }
    }
}