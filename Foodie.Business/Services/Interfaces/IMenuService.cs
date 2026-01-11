using Foodie.Models.ViewModels.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for managing menu data operations.
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Retrieves all available menus.
        /// </summary>
        /// <returns>A collection of <see cref="MenuViewModel"/> objects.</returns>
        Task<IEnumerable<MenuViewModel>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific menu by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the menu.</param>
        /// <returns>A <see cref="MenuViewModel"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<MenuViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all menus for a given restaurant.
        /// </summary>
        /// <param name="restaurantId">The restaurant identifier.</param>
        /// <returns>A collection of <see cref="MenuViewModel"/> objects.</returns>
        Task<IEnumerable<MenuViewModel>> GetByRestaurantIdAsync(Guid restaurantId);

        /// <summary>
        /// Creates a new menu entry in the system.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the menu to create.</param>
        /// <returns>The created <see cref="MenuViewModel"/>.</returns>
        Task<MenuViewModel> CreateAsync(MenuCreateOrEditViewModel model);

        /// <summary>
        /// Updates the details of an existing menu.
        /// </summary>
        /// <param name="id">The unique identifier of the menu to update.</param>
        /// <param name="model">The updated data for the menu.</param>
        /// <returns>The updated <see cref="MenuViewModel"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no menu exists with the provided <paramref name="id"/>.</exception>
        Task<MenuViewModel> UpdateAsync(Guid id, MenuCreateOrEditViewModel model);

        /// <summary>
        /// Removes a menu from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the menu to delete.</param>
        /// <returns>The <see cref="MenuViewModel"/> of the deleted menu.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no menu exists with the provided <paramref name="id"/>.</exception>
        Task<MenuViewModel> DeleteAsync(Guid id);
    }
}
