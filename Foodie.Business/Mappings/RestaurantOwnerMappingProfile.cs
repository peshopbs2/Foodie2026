using AutoMapper;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.Restaurants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Mappings
{
    public class RestaurantOwnerMappingProfile : Profile
    {
        public RestaurantOwnerMappingProfile()
        {
            CreateMap<IdentityUser, RestaurantOwnerDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Restaurant, RestaurantViewModel>()
                .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners.Select(x => x.User)));
        }
    }
}
