using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using IPProje.Extensions;
using IPProje.Extensions.ImageHelpers;
using IPProje.Extensions.Messages;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.Roller;
using IPProje.ViewModels.Articles;
using IPProje.ViewModels.Categories;
using IPProje.ViewModels.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;

namespace IPProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IValidator<Article> validator;
        private readonly IToastNotification toast;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IImageHelper imageHelper;
        private readonly ClaimsPrincipal _user;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper, IValidator<Article> validator, IToastNotification toast, IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validator = validator;
            this.toast = toast;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
            this.imageHelper = imageHelper;
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Index()
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.IsDeleted, x => x.Category);
            var map = mapper.Map<List<ArticleModel>>(articles);

            return View(map);
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Add()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x =>! x.IsDeleted);
            var map = mapper.Map<List<CategoryModel>>(categories);
            return View(new ArticleAddModel { Categories = map });
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Add(ArticleAddModel articleAddModel)
        {
            var articleMap = mapper.Map<Article>(articleAddModel);
            var result = await validator.ValidateAsync(articleMap);

            if (result.IsValid)
            {
                var userId = _user.GetLoggedInUserId();
                var userEmail = _user.GetLoggedInEmail();

                var imageUpload = await imageHelper.Upload(articleAddModel.Title, articleAddModel.Photo, ImageType.Post);
                Image image = new(imageUpload.FullName, articleAddModel.Photo.ContentType, userEmail);
                await  unitOfWork.GetRepository<Image>().AddAsync(image);

                var article = new Article(articleAddModel.Title, articleAddModel.Content, userId, userEmail, articleAddModel.CategoryId, image.Id);

                await unitOfWork.GetRepository<Article>().AddAsync(article);
                await unitOfWork.SaveAsync();
                toast.AddSuccessToastMessage(Messages.Article.Add(articleAddModel.Title), new ToastrOptions { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState);
            }
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted);
            var categoriesMap = mapper.Map<List<CategoryModel>>(categories);
            return View(new ArticleAddModel { Categories = categoriesMap });
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Update(Guid articleId)
        {
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleId, x => x.Category, i => i.Image, u => u.User);
            var articleMap = mapper.Map<ArticleModel>(article);

            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted);
            var categoriesMap = mapper.Map<List<CategoryModel>>(categories);

            var articleUpdateModel = mapper.Map<ArticleUpdateModel>(articleMap);
            articleUpdateModel.Categories = categoriesMap;
            return View(articleUpdateModel);
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Update(ArticleUpdateModel articleUpdateModel)
        {
            var map = mapper.Map<Article>(articleUpdateModel);
            var result = await validator.ValidateAsync(map);
            if (result.IsValid)
            {
                var userEmail = _user.GetLoggedInEmail();
                var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleUpdateModel.Id, x => x.Category, i => i.Image);

                if (articleUpdateModel.Photo != null)
                {
                    imageHelper.Delete(article.Image.FileName);

                    var imageUpload = await imageHelper.Upload(articleUpdateModel.Title, articleUpdateModel.Photo, ImageType.Post);
                    Image image = new(imageUpload.FullName, articleUpdateModel.Photo.ContentType, userEmail);
                    await unitOfWork.GetRepository<Image>().AddAsync(image);

                    article.ImageId = image.Id;
                    await unitOfWork.SaveAsync();
                }

                article.Title = articleUpdateModel.Title;
                article.Content = articleUpdateModel.Content;
                article.CategoryId = articleUpdateModel.CategoryId;
                article.ModifiedDate = DateTime.Now;
                article.ModifiedBy = userEmail;

                await unitOfWork.GetRepository<Article>().UpdateAsync(article);
                await unitOfWork.SaveAsync();
                toast.AddSuccessToastMessage(Messages.Article.Update(articleUpdateModel.Title), new ToastrOptions() { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState);
            }
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted);
            var categoriesMap = mapper.Map<List<CategoryModel>>(categories);
            articleUpdateModel.Categories = categoriesMap;
            return View(articleUpdateModel);
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Delete(Guid articleId)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);

            article.IsDeleted = true;
            article.DeletedDate = DateTime.Now;
            article.DeletedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();
            toast.AddSuccessToastMessage(Messages.Article.Delete(article.Title), new ToastrOptions() { Title = "İşlem Başarılı" });


            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
    }
}
