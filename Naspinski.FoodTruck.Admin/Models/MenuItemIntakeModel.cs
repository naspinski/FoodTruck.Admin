using Naspinski.FoodTruck.Data.Distribution.Models.Menu;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naspinski.FoodTruck.AdminWeb.Models
{
    public class MenuItemIntakeModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? SortOrder { get; set; }
        public string ItemId { get; set; }
        public int? CategoryId { get; set; }
        public List<PriceModel> Prices { get; set; }
        public List<ComboPartModel> ComboParts { get; set; }

        public MenuItemModel ToMenuItemModel()
        {
            return new MenuItemModel()
            { 
                Id = Id,
                Name = Name,
                Description = string.IsNullOrWhiteSpace(Description) ? string.Empty : Description,
                ImageId = ImageId,
                SortOrder = SortOrder ?? 0,
                ItemId = string.IsNullOrWhiteSpace(ItemId) ? string.Empty : ItemId,
                CategoryId = CategoryId,
                Prices = Prices ?? new List<PriceModel>(),
                ComboParts = ComboParts ?? new List<ComboPartModel>()
            };
        }
    }
}
