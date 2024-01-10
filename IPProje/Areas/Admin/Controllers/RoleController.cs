using IPProje.Extensions.Messages;
using IPProje.Models;
using IPProje.Roller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using static IPProje.Extensions.Messages.Messages;

namespace IPProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> roleManager;
        private readonly IToastNotification toast;

        public RoleController(RoleManager<AppRole> roleManager, IToastNotification toast)
        {
            this.roleManager = roleManager;
            this.toast = toast;
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> RoleAddWithAjax([FromBody] AppRole model)
        {
            var role = await roleManager.FindByNameAsync(model.Name);
            if (role == null)
            {
                var newrole = new AppRole();
                newrole.Name = model.Name;
                await roleManager.CreateAsync(newrole);
                toast.AddSuccessToastMessage(Messages.User.Add(model.Name), new ToastrOptions { Title = "İşlem Başarılı" });
            }
            return Json(Messages.Role.Add(model.Name));
        }
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}")]
        public async Task<IActionResult> Delete(Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                toast.AddSuccessToastMessage(Messages.User.Delete(role.Name), new ToastrOptions() { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "Role", new { Area = "Admin" });
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
