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
    /// Represents the data returned when retrieving restaurant
    /// </summary>
    public class RestaurantViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public Guid Id { get; set; }

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
        /// Gets or sets the collection of images associated with the restaurant.
        /// </summary>
        public List<RestaurantImageViewModel> Images { get; set; } = new List<RestaurantImageViewModel>();
        /// <summary>
        /// List of User IDs selected as owners.
        /// </summary>
        public List<RestaurantOwnerDto> Owners { get; set; } = new List<RestaurantOwnerDto>();
    }
}
