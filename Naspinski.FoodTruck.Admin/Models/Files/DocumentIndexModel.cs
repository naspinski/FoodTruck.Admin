using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Naspinski.FoodTruck.Data.Models.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Naspinski.FoodTruck.AdminWeb.Models.Files
{
    public class DocumentIndexModel
    {
        [Required]
        public string Category { get; set; }
        
        [Required]
        public string Type { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public IEnumerable<DocumentModel> Documents;
        public IEnumerable<SelectListItem> Categories
        {
            get
            {
                return typeof(Document.DocCategory)
                    .GetFields()
                    .Select(x => new SelectListItem(x.GetValue(null).ToString(), x.GetValue(null).ToString()));
            }
        }
    }
}
