using Foodie.Business.Services.Interfaces;
using Foodie.Models.ViewModels.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foodie.Web.Controllers
{
    public class MenusController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;

        public MenusController(IMenuService menuService, IRestaurantService restaurantService)
        {
            _menuService = menuService;
            _restaurantService = restaurantService;
        }

        public async Task<IActionResult> Index()
        {
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }

        [HttpGet("Menus/Restaurant/{restaurantId:guid}")]
        public async Task<IActionResult> Restaurant(Guid restaurantId)
        {
            var restaurant = await _restaurantService.GetByIdAsync(restaurantId);
            if (restaurant == null) {
                return NotFound();
            }

            var menus = await _menuService.GetByRestaurantIdAsync(restaurantId);
            var model = new ListMenusByRestaurantIdViewModel()
            {
                Menus = menus,
                RestaurantName = restaurant.Name
            };
            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var menu = await _menuService.GetByIdAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateRestaurantsAsync();

            var model = new MenuCreateOrEditViewModel
            {
                ActiveFrom = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuCreateOrEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRestaurantsAsync(model.RestaurantId);
                return View(model);
            }

            try
            {
                await _menuService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateRestaurantsAsync(model.RestaurantId);
                return View(model);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateRestaurantsAsync(model.RestaurantId);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var menu = await _menuService.GetByIdAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            await PopulateRestaurantsAsync(menu.RestaurantId);

            var model = new MenuCreateOrEditViewModel
            {
                RestaurantId = menu.RestaurantId,
                Name = menu.Name,
                ActiveFrom = menu.ActiveFrom,
                ActiveTo = menu.ActiveTo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MenuCreateOrEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRestaurantsAsync(model.RestaurantId);
                return View(model);
            }

            try
            {
                await _menuService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateRestaurantsAsync(model.RestaurantId);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var menu = await _menuService.GetByIdAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _menuService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private async Task PopulateRestaurantsAsync(Guid? selectedRestaurantId = null)
        {
            var restaurants = await _restaurantService.GetAllAsync();

            ViewBag.Restaurants = new SelectList(
                restaurants.Select(r => new { r.Id, r.Name }),
                "Id",
                "Name",
                selectedRestaurantId
            );
        }
    }
}
