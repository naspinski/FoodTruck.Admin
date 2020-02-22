using Naspinski.FoodTruck.Data.Distribution.Models.System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naspinski.FoodTruck.AdminWeb.Models.Files
{
    public class ImageIndexModel
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public IEnumerable<ImageModel> Images;
    }
}
