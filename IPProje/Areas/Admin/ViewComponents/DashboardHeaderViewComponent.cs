using AutoMapper;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IPProje.Areas.Admin.ViewComponents
{
    public class DashboardHeaderViewComponent : ViewComponent
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public DashboardHeaderViewComponent(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var role = string.Join("", await userManager.GetRolesAsync(user));
            var userId = user.Id;

            var getUserWithImage = await unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == userId, x => x.Image);
            var map = mapper.Map<UserProfileModel>(getUserWithImage);
            map.Role = role;
            map.Image.FileName = getUserWithImage.Image.FileName;

            return View(map);
        }
    }
}
