using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.MenuItems
{
    /// <summary>
    /// Represents a menu item create or edit model.
    /// </summary>
    public class MenuItemCreateOrEditViewModel
    {
        /// <summary>
        /// Gets or sets the menu category id.
        /// </summary>
        [Required]
        public Guid MenuCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        [Required(ErrorMessage = "The item name is required.")]
        [MaxLength(200, ErrorMessage = "The item name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        [MaxLength(1000, ErrorMessage = "The description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the item price.
        /// </summary>
        [Required]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets whether the item is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the item is available (in stock).
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the display order inside the category.
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
