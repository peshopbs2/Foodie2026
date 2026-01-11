using AutoMapper;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Mappings
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantViewModel>()
                .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners.Select(x => x.User)));

            CreateMap<RestaurantCreateOrEditViewModel, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Owners, opt => opt.Ignore());

            CreateMap<RestaurantImage, RestaurantImageViewModel>();

            CreateMap<RestaurantViewModel, RestaurantCreateOrEditViewModel>()
                .ForMember(dest => dest.ExistingImages, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Images, opt => opt.Ignore());
        }
    }
}
