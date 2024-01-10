using AutoMapper;
using IPProje.Extensions;
using IPProje.Extensions.ImageHelpers;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.ViewModels.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;


namespace IPProje.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IImageHelper imageHelper;
        private readonly IToastNotification toast;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public UserController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper, IImageHelper imageHelper, IToastNotification toast, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageHelper = imageHelper;
            this.toast = toast;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var profile = await GetUserProfileAsync();
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> UserProfile(UserProfileModel userProfileModel)
        {
            if (ModelState.IsValid)
            {
                var result = await UserProfileUpdate(userProfileModel);
                if (result)
                {
                    toast.AddSuccessToastMessage("Profil güncelleme işlemi tamamlandı", new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "Home", new { Area = "" });
                }
                else
                {
                    var profile = await GetUserProfileAsync();
                    toast.AddErrorToastMessage("Profil güncelleme işlemi tamamlanamadı", new ToastrOptions { Title = "İşlem Başarısız" });
                    return View(profile);
                }
            }
            else
                return NotFound();
        }

        public async Task<UserProfileModel> GetUserProfileAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var role = string.Join("", await _userManager.GetRolesAsync(user));
            var userId = user.Id;

            var getUserWithImage = await unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == userId, x => x.Image);
            var map = mapper.Map<UserProfileModel>(getUserWithImage);
            map.Role = role;
            map.Image.FileName = getUserWithImage.Image.FileName;

            return map;

        }
        public async Task<AppUser> GetAppUserByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }
        public async Task<Guid> UploadUserPhoto(UserProfileModel userProfileModel)
        {
            var userEmail = _user.GetLoggedInEmail();

            var imageUpload = await imageHelper.Upload($"{userProfileModel.FirstName}{userProfileModel.LastName}", userProfileModel.Photo, ImageType.User);
            Image image = new(imageUpload.FullName, userProfileModel.Photo.ContentType, userEmail);
            await unitOfWork.GetRepository<Image>().AddAsync(image);
            return image.Id;
        }
        [HttpPost]
        public async Task<bool> UserProfileUpdate(UserProfileModel userProfileModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user.Id;
            
            await _userManager.UpdateSecurityStampAsync(user);
            mapper.Map(userProfileModel, user);

            if (userProfileModel.Photo != null)
                user.ImageId = await UploadUserPhoto(userProfileModel);

            await _userManager.UpdateAsync(user);
            await unitOfWork.SaveAsync();
            return true;
        }
    }
}
