using AutoMapper;
using Foodie.Business.Repositories.Interfaces;
using Foodie.Business.Services.Interfaces;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.Menus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Services.Implementations
{
    /// <summary>
    /// Provides menu-related operations.
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly IRepository<Menu> _menuRepository;
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IMapper _mapper;

        public MenuService(IRepository<Menu> menuRepository, IRepository<Restaurant> restaurantRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MenuViewModel>> GetAllAsync()
        {
            var menus = await _menuRepository.GetAllAsync(m => m.Restaurant);
            return _mapper.Map<IEnumerable<MenuViewModel>>(menus);
        }

        public async Task<MenuViewModel?> GetByIdAsync(Guid id)
        {
            var menu = await _menuRepository.Query()
                .Where(m => m.Id == id)
                .Include(m => m.Categories)
                    .ThenInclude(mc => mc.Items)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuViewModel?>(menu);
        }

        public async Task<IEnumerable<MenuViewModel>> GetByRestaurantIdAsync(Guid restaurantId)
        {
            var menus = await _menuRepository.Query()
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MenuViewModel>>(menus);
        }

        public async Task<MenuViewModel> CreateAsync(MenuCreateOrEditViewModel model)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(model.RestaurantId);
            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Restaurant ID {model.RestaurantId} not found.");
            }

            if (model.ActiveTo.HasValue && model.ActiveTo.Value < model.ActiveFrom)
            {
                throw new ArgumentException("ActiveTo cannot be earlier than ActiveFrom.");
            }

            var menu = _mapper.Map<Menu>(model);
            menu.Id = Guid.NewGuid();

            await _menuRepository.AddAsync(menu);
            await _menuRepository.CommitAsync();

            return _mapper.Map<MenuViewModel>(menu);
        }

        public async Task<MenuViewModel> UpdateAsync(Guid id, MenuCreateOrEditViewModel model)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                throw new KeyNotFoundException($"ID {id} not found.");
            }

            var restaurant = await _restaurantRepository.GetByIdAsync(model.RestaurantId);
            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Restaurant ID {model.RestaurantId} not found.");
            }

            if (model.ActiveTo.HasValue && model.ActiveTo.Value < model.ActiveFrom)
            {
                throw new ArgumentException("ActiveTo cannot be earlier than ActiveFrom.");
            }

            _mapper.Map(model, menu);

            _menuRepository.Update(menu);
            await _menuRepository.CommitAsync();

            return _mapper.Map<MenuViewModel>(menu);
        }

        public async Task<MenuViewModel> DeleteAsync(Guid id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                throw new KeyNotFoundException($"ID {id} not found.");
            }

            _menuRepository.Remove(menu);
            await _menuRepository.CommitAsync();

            return _mapper.Map<MenuViewModel>(menu);
        }
    }
}
