using AutoMapper;
using IPProje.Models;
using IPProje.ViewModels.Categories;

namespace IPProje.Extensions.AutoMapper
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryModel, Category>().ReverseMap();
            CreateMap<CategoryAddModel, Category>().ReverseMap();
            CreateMap<CategoryUpdateModel, Category>().ReverseMap();
        }
    }
}
