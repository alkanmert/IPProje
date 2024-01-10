using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using IPProje.Extensions;
using IPProje.Extensions.Messages;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.Roller;
using IPProje.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;

namespace IPProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IToastNotification toast;
        private readonly IValidator<Category> validator;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public CategoryController(IUnitOfWork unitOfWork,IMapper mapper, IToastNotification toast, IValidator<Category> validator, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.toast = toast;
            this.validator = validator;
            _user = httpContextAccessor.HttpContext.User;
            this.httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Index()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted);
            var map = mapper.Map<List<CategoryModel>>(categories);
            return View(map);
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> AddWithAjaxMethod([FromBody] CategoryAddModel categoryAddModel)
        {
            var map = mapper.Map<Category>(categoryAddModel);
            var result = await validator.ValidateAsync(map);

            if (result.IsValid)
            {
                var userEmail = _user.GetLoggedInEmail();
                Category category = new Category(categoryAddModel.Name, userEmail);

                await unitOfWork.GetRepository<Category>().AddAsync(category);
                await unitOfWork.SaveAsync();

                toast.AddSuccessToastMessage(Messages.Category.Add(categoryAddModel.Name), new ToastrOptions { Title = "İşlem Başarılı" });

                return Json(Messages.Category.Add(categoryAddModel.Name));
            }
            else
            {
                toast.AddErrorToastMessage(result.Errors.First().ErrorMessage, new ToastrOptions { Title = "İşlem Başarısız" });
                return Json(result.Errors.First().ErrorMessage);
            }
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Update(Guid categoryId)
        {
            var category = await unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);
            var map = mapper.Map<Category, CategoryUpdateModel>(category);

            return View(map);
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public async Task<IActionResult> Update(CategoryUpdateModel categoryUpdateModel)
        {
            var map = mapper.Map<Category>(categoryUpdateModel);
            var result = await validator.ValidateAsync(map);

            if (result.IsValid)
            {
                var userName = _user.GetLoggedInEmail();
                var category = await unitOfWork.GetRepository<Category>().GetAsync(x => !x.IsDeleted && x.Id == categoryUpdateModel.Id);

                category.Name = categoryUpdateModel.Name;
                category.ModifiedBy = userName;
                category.ModifiedDate = DateTime.Now;

                await unitOfWork.GetRepository<Category>().UpdateAsync(category);
                await unitOfWork.SaveAsync();

                toast.AddSuccessToastMessage(Messages.Category.Update(categoryUpdateModel.Name), new ToastrOptions { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }
            result.AddToModelState(this.ModelState);
            return View();
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Delete(Guid categoryId)
        {
            var userName = _user.GetLoggedInEmail();
            var category = await unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);

            category.IsDeleted = true;
            category.DeletedBy = userName;
            category.DeletedDate = DateTime.Now;

            await unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await unitOfWork.SaveAsync();

            toast.AddSuccessToastMessage(Messages.Category.Delete(category.Name), new ToastrOptions { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
    }
}
