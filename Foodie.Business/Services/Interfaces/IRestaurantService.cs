using Foodie.Models.ViewModels.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for managing restaurant data operations.
    /// </summary>
    public interface IRestaurantService
    {
        /// <summary>
        /// Retrieves all available restaurants.
        /// </summary>
        /// <returns>A collection of <see cref="RestaurantViewModel"/> objects.</returns>
        Task<IEnumerable<RestaurantViewModel>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific restaurant by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the restaurant.</param>
        /// <returns>
        /// A <see cref="RestaurantViewModel"/> if found; otherwise, <see langword="null"/>.
        /// </returns>
        Task<RestaurantViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Searches for restaurants that match a specific address or location string.
        /// </summary>
        /// <param name="address">The address or partial address to search for.</param>
        /// <returns>A collection of restaurants matching the search criteria.</returns>
        Task<IEnumerable<RestaurantViewModel>> SearchByAddressAsync(string address);

        /// <summary>
        /// Creates a new restaurant entry in the system.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the restaurant to create.</param>
        /// <returns>The created <see cref="RestaurantViewModel"/>.</returns>
        Task<RestaurantViewModel> CreateAsync(RestaurantCreateOrEditViewModel model);

        /// <summary>
        /// Updates the details of an existing restaurant.
        /// </summary>
        /// <param name="id">The unique identifier of the restaurant to update.</param>
        /// <param name="model">The updated data for the restaurant.</param>
        /// <returns>The updated <see cref="RestaurantViewModel"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no restaurant exists with the provided <paramref name="id"/>.</exception>
        Task<RestaurantViewModel> UpdateAsync(Guid id, RestaurantCreateOrEditViewModel model);

        /// <summary>
        /// Removes a restaurant from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the restaurant to delete.</param>
        /// <returns>The <see cref="RestaurantViewModel"/> of the deleted restaurant.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no restaurant exists with the provided <paramref name="id"/>.</exception>
        Task<RestaurantViewModel> DeleteAsync(Guid id);
    }
}
