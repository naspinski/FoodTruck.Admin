using Naspinski.FoodTruck.Data.Distribution.Models.System;
using System.Collections.Generic;
using System.Linq;

namespace Naspinski.FoodTruck.AdminWeb.Models.DeliveryDestination
{
    public class DeliveryDestinationsIndexModel
    {
        public DeliveryDestinationCrudModel City;
        public DeliveryDestinationCrudModel ZipCode;

        public DeliveryDestinationsIndexModel(IEnumerable<DeliveryDestinationModel> models)
        {
            models = models.OrderBy(x => x.Value);

            City = new DeliveryDestinationCrudModel(models, true);
            ZipCode = new DeliveryDestinationCrudModel(models, false);
        }
    }
}
