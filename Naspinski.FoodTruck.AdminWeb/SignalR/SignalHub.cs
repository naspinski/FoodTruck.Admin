using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naspinski.FoodTruck.AdminWeb.SignalR
{
    public class SignalHub : Hub<ISignalHub>
    {
        public Task Broadcast(string message)
        {
            return Clients.All.Broadcast(message);
        }
    }
    
    public interface ISignalHub
    {
        Task Broadcast(string message);
    }
}
