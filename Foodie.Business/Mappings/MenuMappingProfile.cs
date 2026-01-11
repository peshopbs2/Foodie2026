using AutoMapper;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Mappings
{
    /// <summary>
    /// Defines mappings for menu view models.
    /// </summary>
    public class MenuMappingProfile : Profile
    {
        public MenuMappingProfile()
        {
            CreateMap<Menu, MenuViewModel>()
                .ForMember(m => m.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name));
            CreateMap<MenuCreateOrEditViewModel, Menu>();
        }
    }
}
