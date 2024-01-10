using AutoMapper;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IPProje.ViewComponents
{
    public class HomeUserViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public HomeUserViewComponent(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(User.Identity.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var role = string.Join("", await _userManager.GetRolesAsync(user));
                var userId = user.Id;

                var getUserWithImage = await unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == userId, x => x.Image);
                var map = mapper.Map<UserProfileModel>(getUserWithImage);
                map.Role = role;
                map.Image.FileName = getUserWithImage.Image.FileName;

                return View(map);

            }
            else
            {
                return View();
            }

        }
    }
}
