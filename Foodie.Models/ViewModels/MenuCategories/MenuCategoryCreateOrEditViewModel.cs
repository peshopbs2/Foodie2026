using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.MenuCategories
{
    /// <summary>
    /// Represents a menu category create or edit model.
    /// </summary>
    public class MenuCategoryCreateOrEditViewModel
    {
        /// <summary>
        /// Gets or sets the menu id to which the category will belong.
        /// </summary>
        [Required]
        public Guid MenuId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [Required(ErrorMessage = "The category name is required.")]
        [MaxLength(100, ErrorMessage = "The category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display order inside the menu.
        /// </summary>
        [Required]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets whether the category is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
