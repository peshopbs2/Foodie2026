using Foodie.Models.ViewModels.MenuCategories;
using Foodie.Models.ViewModels.MenuItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.Menus
{
    /// <summary>
    /// Represents a menu view model.
    /// </summary>
    public class MenuViewModel
    {
        /// <summary>
        /// Gets or sets the menu id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the restaurant id.
        /// </summary>
        public Guid RestaurantId { get; set; }

        /// <summary>
        /// Gets or sets the restaurant name.
        /// </summary>
        public string RestaurantName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the menu name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start of the menu activity period.
        /// </summary>
        public DateTime ActiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the end of the menu activity period.
        /// </summary>
        public DateTime? ActiveTo { get; set; }

        /// <summary>
        /// Gets or sets the list of categories in this menu (optional).
        /// </summary>
        public List<MenuCategoryViewModel> Categories { get; set; } = new();
    }
}
