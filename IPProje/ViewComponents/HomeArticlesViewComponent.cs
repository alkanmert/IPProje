using AutoMapper;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.ViewModels.Articles;
using IPProje.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using static IPProje.Extensions.Messages.Messages;
using Article = IPProje.Models.Article;

namespace IPProje.ViewComponents
{
    public class HomeArticlesViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public HomeArticlesViewComponent(IUnitOfWork unitOfWork, IMapper mapper )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? categoryId)
        {
            var articles = categoryId == null
                            ? await unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted, a => a.Category, i => i.Image, u => u.User)
                            : await unitOfWork.GetRepository<Article>().GetAllAsync(a => a.CategoryId == categoryId && !a.IsDeleted,
                                a => a.Category, i => i.Image, u => u.User);

            var map = new ArticleListModel
            {
                Articles = articles,
                CategoryId = categoryId == null ? null : categoryId.Value,
            };

            return View(map);
        }
    }
}
