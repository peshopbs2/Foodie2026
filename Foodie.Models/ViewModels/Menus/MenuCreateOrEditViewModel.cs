using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.Menus
{
    /// <summary>
    /// Represents a menu create or edit model.
    /// </summary>
    public class MenuCreateOrEditViewModel
    {
        /// <summary>
        /// Gets or sets the restaurant id to which the menu belongs.
        /// </summary>
        [Required]
        public Guid RestaurantId { get; set; }

        /// <summary>
        /// Gets or sets the menu name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start of the menu activity period.
        /// </summary>
        [Required]
        public DateTime ActiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the end of the menu activity period.
        /// </summary>
        public DateTime? ActiveTo { get; set; }
    }
}
