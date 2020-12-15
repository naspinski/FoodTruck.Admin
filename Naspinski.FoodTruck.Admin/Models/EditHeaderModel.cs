using Naspinski.FoodTruck.Admin.Extensions;

namespace Naspinski.FoodTruck.Admin.Models
{
    public class EditHeaderModel
    {
        public bool IsNew;
        public string Controller;
        public string Text => Controller.CamelCaseToSpacedString();

        public EditHeaderModel(bool isNew, string controller) => (IsNew, Controller) = (isNew, controller);
    }
}
