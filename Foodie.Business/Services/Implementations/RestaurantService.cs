using AutoMapper;
using Foodie.Business.Repositories.Interfaces;
using Foodie.Business.Services.Interfaces;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.Restaurants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Services.Implementations
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<RestaurantImage> _restaurantImagesRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public RestaurantService(IRepository<Restaurant> restaurantRepository, IRepository<RestaurantImage> restaurantImagesRepository, IImageService imageService, IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantImagesRepository = restaurantImagesRepository;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RestaurantViewModel>> GetAllAsync()
        {  
            var restaurants = await _restaurantRepository.GetAllAsync(r => r.Images, r => r.Owners);
            return _mapper.Map<IEnumerable<RestaurantViewModel>>(restaurants);
        }

        public async Task<RestaurantViewModel?> GetByIdAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.Query()
                .Where(r => r.Id == id)
                .Include(r => r.Images)
                .Include(r => r.Owners)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync();
            return _mapper.Map<RestaurantViewModel?>(restaurant);
        }

        public async Task<IEnumerable<RestaurantViewModel>> SearchByAddressAsync(string address)
        {
            var restaurants = await _restaurantRepository.GetAllAsync(r => r.Images);
            var filtered = restaurants.Where(r => r.Address.Contains(address, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<IEnumerable<RestaurantViewModel>>(filtered);
        }

        public async Task<RestaurantViewModel> CreateAsync(RestaurantCreateOrEditViewModel model)
        {
            var restaurant = _mapper.Map<Restaurant>(model);
            restaurant.Id = Guid.NewGuid();

            //process images
            if (model.Images != null && model.Images.Any())
            {
                for (int i = 0; i < model.Images.Count; i++)
                {
                    var file = model.Images[i];

                    // Upload the file using the image service
                    var imagePath = await _imageService.UploadImageAsync(file, "restaurants");

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        restaurant.Images.Add(new RestaurantImage
                        {
                            Id = Guid.NewGuid(),
                            ImagePath = imagePath,
                            RestaurantId = restaurant.Id,
                            IsMainImage = (i == model.MainImageIndex)
                        });
                    }
                }

                // Fallback: If no image is selected as main
                if (restaurant.Images.Any() && !restaurant.Images.Any(x => x.IsMainImage))
                {
                    restaurant.Images.First().IsMainImage = true;
                }
            }

            if (model.SelectedOwnerIds != null)
            {
                foreach (var userId in model.SelectedOwnerIds)
                {
                    restaurant.Owners.Add(new RestaurantOwner
                    {
                        UserId = userId
                    });
                }
            }

            await _restaurantRepository.AddAsync(restaurant);
            await _restaurantRepository.CommitAsync();

            return _mapper.Map<RestaurantViewModel>(restaurant);
        }

        public async Task<RestaurantViewModel> UpdateAsync(Guid id, RestaurantCreateOrEditViewModel model)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id, r => r.Images, r => r.Owners);
            if (restaurant == null)
            {
                throw new KeyNotFoundException($"ID {id} not found.");
            }

            // This maps the model properties directly onto the existing 'restaurant' instance
            _mapper.Map(model, restaurant);

            //process images
            if (model.Images != null && model.Images.Any())
            {
                for (int i = 0; i < model.Images.Count; i++)
                {
                    var file = model.Images[i];
                    var imagePath = await _imageService.UploadImageAsync(file, "restaurants");

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        await _restaurantImagesRepository.AddAsync(new RestaurantImage
                        {
                            Id = Guid.NewGuid(),
                            ImagePath = imagePath,
                            RestaurantId = restaurant.Id,
                            IsMainImage = (i == model.MainImageIndex)
                        });
                    }
                }
            }

            var ownersToRemove = restaurant.Owners
                .Where(o => !model.SelectedOwnerIds.Contains(o.UserId))
                .ToList();

            foreach (var owner in ownersToRemove)
            {
                restaurant.Owners.Remove(owner);
            }

            foreach (var userId in model.SelectedOwnerIds)
            {
                if (!restaurant.Owners.Any(o => o.UserId == userId))
                {
                    restaurant.Owners.Add(new RestaurantOwner
                    {
                        RestaurantId = restaurant.Id,
                        UserId = userId
                    });
                }
            }

            _restaurantRepository.Update(restaurant);
            await _restaurantRepository.CommitAsync();

            return _mapper.Map<RestaurantViewModel>(restaurant);
        }

        public async Task<RestaurantViewModel> DeleteAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id, r => r.Images);
            if (restaurant == null)
            {
                throw new KeyNotFoundException($"ID {id} not found.");
            }

            if (restaurant.Images != null)
            {
                foreach (var img in restaurant.Images)
                {
                    _imageService.DeleteImage(img.ImagePath);
                }
            }

            _restaurantRepository.Remove(restaurant);
            await _restaurantRepository.CommitAsync();

            return _mapper.Map<RestaurantViewModel>(restaurant);
        }
    }
}
