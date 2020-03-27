using Naspinski.FoodTruck.Data.Models.System;
using System.ComponentModel.DataAnnotations;

namespace Naspinski.FoodTruck.AdminWeb.Models.Settings
{
    public class SettingModel
    {
        [Required]
        public string Name { get; set; }
        public string DisplayName
        {
            get
            {
                return System.Text.RegularExpressions.Regex.Replace(
                    System.Text.RegularExpressions.Regex.Replace(Name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
        }

        public string Regex;
        public string Category;
        public string DateType;
        public bool IsHidden;
        
        public string Value { get; set; }

        public SettingModel() { }
        public SettingModel(Setting model)
        {
            Name = model.Name;
            Value = model.Value;
            DateType = model.DataType;
            Regex = model.Regex;
            Category = model.Category;
            IsHidden = model.IsHidden;
        }

        public Setting ToModel()
        {
            return new Setting()
            {
                Name = Name,
                Value = Value
            };
        }
    }
}
