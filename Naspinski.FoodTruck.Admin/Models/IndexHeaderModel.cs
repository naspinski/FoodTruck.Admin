namespace Naspinski.FoodTruck.Admin.Models
{
    public class IndexHeaderModel
    {
        public string Controller;
        public string Text;

        public IndexHeaderModel(string controller, string text) => (Controller, Text) = (controller, text);
    }
}
