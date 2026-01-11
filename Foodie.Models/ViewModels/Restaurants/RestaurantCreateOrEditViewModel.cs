using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.Restaurants
{
    /// <summary>
    /// Represents the data required when creating restaurant
    /// </summary>
    public class RestaurantCreateOrEditViewModel
    {
        /// <summary>
        /// Gets or sets the restaurant's name
        /// </summary>
        [MinLength(3)]
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the restaurant's address
        /// </summary>
        [MinLength(3)]
        [MaxLength(150)]
        [Required]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the restaurant's phone number
        /// </summary>
        [MinLength(6)]
        [MaxLength(20)]
        [Required]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the restaurant's capacity
        /// </summary>
        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        /// <summary>
        /// Images uploaded by the user
        /// </summary>
        [Display(Name = "Restaurant Images")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        /// <summary>
        /// Index of the image to be Main (Example: 0 for the first one)
        /// </summary>
        public int MainImageIndex { get; set; } = 0;

        public List<RestaurantImageViewModel> ExistingImages { get; set; } = new List<RestaurantImageViewModel>();

        /// <summary>
        /// List of User IDs selected as owners.
        /// </summary>
        public List<string> SelectedOwnerIds { get; set; } = new List<string>();

        /// <summary>
        /// Helper list to populate the dropdown in the View.
        /// </summary>
        public List<SelectListItem> AvailableOwners { get; set; } = new List<SelectListItem>();
    }
}
