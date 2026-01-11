using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.Restaurants
{
    /// <summary>
    /// Represents a view model for a single restaurant image.
    /// Used for displaying image data in the UI.
    /// </summary>
    public class RestaurantImageViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the image.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the relative file path or URL to the image (e.g., "/images/restaurants/pic.jpg").
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this image is designated as the main/cover image.
        /// </summary>
        public bool IsMainImage { get; set; }
    }
}
