using AutoMapper;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.MenuItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Mappings
{
    /// <summary>
    /// Defines mappings for menu item view models.
    /// </summary>
    public class MenuItemMappingProfile : Profile
    {
        public MenuItemMappingProfile()
        {
            CreateMap<MenuItem, MenuItemViewModel>()
                .ForMember(dest => dest.MenuCategoryName, opt => opt.MapFrom(src => src.MenuCategory.Name));

            CreateMap<MenuItemCreateOrEditViewModel, MenuItem>();
        }
    }
}
