using Naspinski.FoodTruck.Data.Distribution.Models.System;
using System.Collections.Generic;
using System.Linq;

namespace Naspinski.FoodTruck.AdminWeb.Models.DeliveryDestination
{
    public class DeliveryDestinationCrudModel
    {
        public List<DeliveryDestinationModel> Destinations = new List<DeliveryDestinationModel>();

        public DeliveryDestinationModel NewDestination = new DeliveryDestinationModel();

        public DeliveryDestinationCrudModel(IEnumerable<DeliveryDestinationModel> models, bool isCity)
        {
            Destinations = models.Where(x => x.IsCity == isCity).ToList();
            NewDestination.IsCity = isCity;
        }
    }
}
