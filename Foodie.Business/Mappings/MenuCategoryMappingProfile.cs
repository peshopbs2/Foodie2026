using AutoMapper;
using Foodie.Models.Domain.Entities;
using Foodie.Models.ViewModels.MenuCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Business.Mappings
{
    /// <summary>
    /// Defines mappings for menu category view models.
    /// </summary>
    public class MenuCategoryMappingProfile : Profile
    {
        public MenuCategoryMappingProfile()
        {
            CreateMap<MenuCategory, MenuCategoryViewModel>();

            CreateMap<MenuCategoryCreateOrEditViewModel, MenuCategory>();
        }
    }
}
