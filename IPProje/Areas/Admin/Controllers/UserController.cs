using AutoMapper;
using IPProje.Extensions.ImageHelpers;
using IPProje.Extensions.Messages;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.Roller;
using IPProje.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IPProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;
        private readonly IImageHelper imageHelper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IToastNotification toast;
        private readonly ClaimsPrincipal _user;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IImageHelper imageHelper, IHttpContextAccessor httpContextAccessor, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IToastNotification toast)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
            this.imageHelper = imageHelper;
            this.httpContextAccessor = httpContextAccessor;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.toast = toast;
            _user = httpContextAccessor.HttpContext.User;
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var map = mapper.Map<List<UserModel>>(users);

            foreach (var item in map)
            {
                var findUser = await userManager.FindByIdAsync(item.Id.ToString());
                var role = string.Join("", await userManager.GetRolesAsync(findUser));

                item.Role = role;
            }
            return View(map);
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Add()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(new UserAddModel { Roles = roles });
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Add(UserAddModel userAddModel)
        {
            var map = mapper.Map<AppUser>(userAddModel);
            var roles = await roleManager.Roles.ToListAsync();

            if (ModelState.IsValid)
            {
                map.UserName = userAddModel.UserName;
                var result = await userManager.CreateAsync(map, userAddModel.Password);
                if (result.Succeeded)
                {
                    var findRole = await roleManager.FindByIdAsync(userAddModel.RoleId.ToString());
                    await userManager.AddToRoleAsync(map, findRole.ToString());
                    toast.AddSuccessToastMessage(Messages.User.Add(userAddModel.Email), new ToastrOptions() { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    foreach (var errors in result.Errors)
                        ModelState.AddModelError("", errors.Description);

                    return View(new UserAddModel { Roles = roles });
                }
            }
            return View(new UserAddModel { Roles = roles });
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Update(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            var roles = await roleManager.Roles.ToListAsync();

            var map = mapper.Map<UserUpdateModel>(user);
            map.Roles = roles;

            return View(map);
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Update(UserUpdateModel userUpdateModel)
        {
            var user = await userManager.FindByIdAsync(userUpdateModel.Id.ToString());
            if (user != null)
            {
                var userRole = string.Join("", await userManager.GetRolesAsync(user));
                var roles = await roleManager.Roles.ToListAsync();
                if (ModelState.IsValid)
                {
                    mapper.Map(userUpdateModel, user);
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.RemoveFromRoleAsync(user, userRole);
                        var findRole = await roleManager.FindByIdAsync(userUpdateModel.RoleId.ToString());
                        await userManager.AddToRoleAsync(user, findRole.Name);
                        toast.AddSuccessToastMessage(Messages.User.Update(userUpdateModel.Email), new ToastrOptions() { Title = "İşlem Başarılı" });
                        return RedirectToAction("Index", "User", new { Area = "Admin" });
                    }
                    else
                    {
                        foreach (var errors in result.Errors)
                            ModelState.AddModelError("", errors.Description);

                        return View(new UserUpdateModel { Roles = roles });
                    }
                }
            }
            return NotFound();
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                toast.AddSuccessToastMessage(Messages.User.Delete(user.Email), new ToastrOptions() { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
            {
                foreach (var errors in result.Errors)
                    ModelState.AddModelError("", errors.Description);
            }
            return NotFound();
        }
    }
}
