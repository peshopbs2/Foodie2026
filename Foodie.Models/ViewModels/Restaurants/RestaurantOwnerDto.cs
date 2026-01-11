using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models.ViewModels.Restaurants
{
    /// <summary>
    /// Simple Data Transfer Object to display owner info.
    /// </summary>
    public class RestaurantOwnerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
