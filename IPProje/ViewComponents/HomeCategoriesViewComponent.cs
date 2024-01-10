using AutoMapper;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace IPProje.ViewComponents
{
    public class HomeCategoriesViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public HomeCategoriesViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted);
            var map = mapper.Map<List<CategoryModel>>(categories);

            return View(map);
        }
    }
}

