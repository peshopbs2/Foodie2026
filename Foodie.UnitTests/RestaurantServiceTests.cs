using AutoMapper;
using FluentAssertions;
using Foodie.Business.Repositories.Interfaces;
using Foodie.Business.Services.Implementations;
using Foodie.Business.Services.Interfaces;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.Restaurants;
using Microsoft.AspNetCore.Http;
using MockQueryable;
using MockQueryable.NSubstitute;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Foodie.Business.Tests.Services
{
    public class RestaurantServiceTests
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<RestaurantImage> _restaurantImagesRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly RestaurantService _service;

        public RestaurantServiceTests()
        {
            _restaurantRepository = Substitute.For<IRepository<Restaurant>>();
            _restaurantImagesRepository = Substitute.For<IRepository<RestaurantImage>>();
            _imageService = Substitute.For<IImageService>();
            _mapper = Substitute.For<IMapper>();

            _service = new RestaurantService(
                _restaurantRepository,
                _restaurantImagesRepository,
                _imageService,
                _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedRestaurant_WhenFound()
        {
            // Arrange
            var restaurantId = Guid.NewGuid();

            var restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = restaurantId,
                    Name = "Test Restaurant",
                    Images = new List<RestaurantImage> { new RestaurantImage { Id = Guid.NewGuid() } },
                    Owners = new List<RestaurantOwner> { new RestaurantOwner { UserId = "user1" } }
                },
                new Restaurant { Id = Guid.NewGuid(), Name = "Other Restaurant" }
            };

            var mockQueryable = restaurants.BuildMock();

            _restaurantRepository.Query().Returns(mockQueryable);

            _mapper.Map<RestaurantViewModel?>(Arg.Is<Restaurant>(r => r.Id == restaurantId))
                .Returns(new RestaurantViewModel { Id = restaurantId, Name = "Test Restaurant" });

            // Act
            var result = await _service.GetByIdAsync(restaurantId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(restaurantId);

            _restaurantRepository.Received(1).Query();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var restaurants = new List<Restaurant>();
            var mockQueryable = restaurants.BuildMock();

            _restaurantRepository.Query().Returns(mockQueryable);

            // Act
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedRestaurants_WhenRepositoryReturnsData()
        {
            // Arrange
            var restaurants = new List<Restaurant> { new Restaurant { Id = Guid.NewGuid() } };
            var viewModels = new List<RestaurantViewModel> { new RestaurantViewModel() };

            _restaurantRepository.GetAllAsync(Arg.Any<Expression<Func<Restaurant, object>>[]>())
                .Returns(restaurants);

            _mapper.Map<IEnumerable<RestaurantViewModel>>(restaurants).Returns(viewModels);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task SearchByAddressAsync_ShouldReturnFilteredResults_WhenAddressMatches()
        {
            // Arrange
            var searchAddress = "Sofia";
            var restaurants = new List<Restaurant>
            {
                new Restaurant { Id = Guid.NewGuid(), Address = "Sofia, Center" },
                new Restaurant { Id = Guid.NewGuid(), Address = "Plovdiv, Center" }
            };

            _restaurantRepository.GetAllAsync(Arg.Any<Expression<Func<Restaurant, object>>[]>())
                .Returns(restaurants);

            _mapper.Map<IEnumerable<RestaurantViewModel>>(Arg.Is<IEnumerable<Restaurant>>(x => x.Count() == 1))
                .Returns(new List<RestaurantViewModel> { new RestaurantViewModel() });

            // Act
            var result = await _service.SearchByAddressAsync(searchAddress);

            // Assert
            // Мапърът трябва да е извикан само със записа от София
            _mapper.Received(1).Map<IEnumerable<RestaurantViewModel>>(Arg.Is<IEnumerable<Restaurant>>(x => x.First().Address.Contains("Sofia")));
        }

        [Fact]
        public async Task CreateAsync_ShouldUploadImagesAndSetMainImage_WhenImagesProvided()
        {
            // Arrange
            var model = new RestaurantCreateOrEditViewModel
            {
                Images = new List<IFormFile> { Substitute.For<IFormFile>(), Substitute.For<IFormFile>() },
                MainImageIndex = 1, // Втората снимка е главна
                SelectedOwnerIds = new List<string> { "user1" }
            };

            var restaurantEntity = new Restaurant { Id = Guid.NewGuid(), Images = new List<RestaurantImage>(), Owners = new List<RestaurantOwner>() };

            _mapper.Map<Restaurant>(model).Returns(restaurantEntity);
            _imageService.UploadImageAsync(Arg.Any<IFormFile>(), "restaurants").Returns("path/to/image.jpg");
            _mapper.Map<RestaurantViewModel>(restaurantEntity).Returns(new RestaurantViewModel());

            // Act
            await _service.CreateAsync(model);

            // Assert
            await _imageService.Received(2).UploadImageAsync(Arg.Any<IFormFile>(), "restaurants");
            await _restaurantRepository.Received(1).AddAsync(restaurantEntity);
            await _restaurantRepository.Received(1).CommitAsync();

            restaurantEntity.Images.Should().HaveCount(2);
            restaurantEntity.Images.ElementAt(0).IsMainImage.Should().BeFalse();
            restaurantEntity.Images.ElementAt(1).IsMainImage.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_ShouldSetFirstImageAsMain_WhenNoMainImageSelected()
        {
            // Arrange
            var model = new RestaurantCreateOrEditViewModel
            {
                Images = new List<IFormFile> { Substitute.For<IFormFile>() },
                MainImageIndex = -1
            };

            var restaurantEntity = new Restaurant { Id = Guid.NewGuid(), Images = new List<RestaurantImage>() };
            _mapper.Map<Restaurant>(model).Returns(restaurantEntity);
            _imageService.UploadImageAsync(Arg.Any<IFormFile>(), Arg.Any<string>()).Returns("some/path");
            _mapper.Map<RestaurantViewModel>(restaurantEntity).Returns(new RestaurantViewModel());

            // Act
            await _service.CreateAsync(model);

            // Assert
            restaurantEntity.Images.First().IsMainImage.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _restaurantRepository.GetByIdAsync(id, Arg.Any<Expression<Func<Restaurant, object>>[]>())
                .Returns((Restaurant)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateAsync(id, new RestaurantCreateOrEditViewModel()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldRemoveUnselectedOwners_AndAddNewOnes()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingOwnerId = "owner-existing";
            var removedOwnerId = "owner-to-remove";
            var newOwnerId = "owner-new";

            var restaurant = new Restaurant
            {
                Id = id,
                Owners = new List<RestaurantOwner>
                {
                    new RestaurantOwner { UserId = existingOwnerId },
                    new RestaurantOwner { UserId = removedOwnerId }
                }
            };

            var model = new RestaurantCreateOrEditViewModel
            {
                SelectedOwnerIds = new List<string> { existingOwnerId, newOwnerId }
            };

            _restaurantRepository.GetByIdAsync(id, Arg.Any<Expression<Func<Restaurant, object>>[]>())
                .Returns(restaurant);

            _mapper.Map<RestaurantViewModel>(restaurant).Returns(new RestaurantViewModel());

            // Act
            await _service.UpdateAsync(id, model);

            // Assert
            _mapper.Received(1).Map(model, restaurant);

            restaurant.Owners.Should().HaveCount(2);
            restaurant.Owners.Should().Contain(o => o.UserId == existingOwnerId);
            restaurant.Owners.Should().Contain(o => o.UserId == newOwnerId);
            restaurant.Owners.Should().NotContain(o => o.UserId == removedOwnerId);

            _restaurantRepository.Received(1).Update(restaurant);
            await _restaurantRepository.Received(1).CommitAsync();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteImagesAndRemoveRestaurant()
        {
            // Arrange
            var id = Guid.NewGuid();
            var restaurant = new Restaurant
            {
                Id = id,
                Images = new List<RestaurantImage>
                {
                    new RestaurantImage { ImagePath = "img1.jpg" },
                    new RestaurantImage { ImagePath = "img2.jpg" }
                }
            };

            _restaurantRepository.GetByIdAsync(id, Arg.Any<Expression<Func<Restaurant, object>>[]>())
                .Returns(restaurant);

            _mapper.Map<RestaurantViewModel>(restaurant).Returns(new RestaurantViewModel());

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _imageService.Received(1).DeleteImage("img1.jpg");
            _imageService.Received(1).DeleteImage("img2.jpg");
            _restaurantRepository.Received(1).Remove(restaurant);
            await _restaurantRepository.Received(1).CommitAsync();
        }
    }
}