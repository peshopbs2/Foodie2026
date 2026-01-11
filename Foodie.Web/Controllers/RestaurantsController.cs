using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Foodie.Data.Persistance;
using Foodie.Models.Domain.Entities;
using Foodie.Business.Services.Interfaces;
using Foodie.Models.ViewModels.Restaurants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Foodie.Web.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public RestaurantsController(IRestaurantService restaurantService, IMapper mapper, UserManager<IdentityUser> userManager, IAuthorizationService authorizationSevice)
        {
            _restaurantService = restaurantService;
            _mapper = mapper;
            _userManager = userManager;
            _authorizationService = authorizationSevice;
        }

        // GET: Restaurants
        public async Task<IActionResult> Index()
        {
            return View(await _restaurantService.GetAllAsync());
        }

        [Authorize(Roles ="Admin,RestaurantOwner")]
        public async Task<IActionResult> Manage()
        {
            return View(await _restaurantService.GetByOwnerIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        // GET: Restaurants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _restaurantService.GetByIdAsync(id.Value);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        [Authorize(Roles = "Admin,RestaurantOwner")]
        // GET: Restaurants/Create
        public async Task<IActionResult> CreateAsync()
        {
            var model = new RestaurantCreateOrEditViewModel();
            model.AvailableOwners = await GetOwnersSelectList();
            return View(model);
        }

        // POST: Restaurants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestaurantCreateOrEditViewModel restaurant)
        {
            if (ModelState.IsValid)
            {
                await _restaurantService.CreateAsync(restaurant);
                return RedirectToAction(nameof(Index));
            }

            restaurant.AvailableOwners = await GetOwnersSelectList();
            return View(restaurant);
        }

        // GET: Restaurants/Edit/5
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AuthorizationResult authResult = await _authorizationService.AuthorizeAsync(User, id, "RestaurantAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var restaurant = await _restaurantService.GetByIdAsync(id.Value);
            if (restaurant == null)
            {
                return NotFound();
            }
            var editModel = _mapper.Map<RestaurantCreateOrEditViewModel>(restaurant);
            editModel.SelectedOwnerIds = restaurant.Owners
                .Select(o => o.Id)
                .ToList();

            editModel.AvailableOwners = await GetOwnersSelectList();
            return View(editModel);
        }

        // POST: Restaurants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RestaurantCreateOrEditViewModel restaurant)
        {
            AuthorizationResult authResult = await _authorizationService.AuthorizeAsync(User, id, "RestaurantAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var existing = await _restaurantService.GetByIdAsync(id);
                if (existing != null)
                {
                    restaurant.ExistingImages = existing.Images?.ToList() ?? new List<RestaurantImageViewModel>();
                }

                try
                {
                    await _restaurantService.UpdateAsync(id, restaurant);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            restaurant.AvailableOwners = await GetOwnersSelectList();
            return View(restaurant);
        }

        // GET: Restaurants/Delete/5
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AuthorizationResult authResult = await _authorizationService.AuthorizeAsync(User, id, "RestaurantAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var restaurant = await _restaurantService.GetByIdAsync(id.Value);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            AuthorizationResult authResult = await _authorizationService.AuthorizeAsync(User, id, "RestaurantAccessPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant != null)
            {
                await _restaurantService.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(Guid id)
        {
            return _restaurantService.GetByIdAsync(id) != null;
        }

        private async Task<List<SelectListItem>> GetOwnersSelectList()
        {
            var owners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");

            return owners.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            }).ToList();
        }
    }
}
