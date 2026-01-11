using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.Menus
{
    public class ListMenusByRestaurantIdViewModel
    {
        public IEnumerable<MenuViewModel> Menus { get; set; } = new List<MenuViewModel>();
        public string RestaurantName { get; set; } = string.Empty;
    }
}
